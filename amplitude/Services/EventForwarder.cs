
namespace amplitude.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Threading.Channels;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using CloudNative.CloudEvents;
    using System.Net.Http;
    using System.Net.Http.Json;
    using amplitude.Models;
    using Microsoft.Extensions.Configuration;
    using CloudNative.CloudEvents.SystemTextJson;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using amplitude.Serialization;

    public interface IEventForwarderQueue
    {
        ValueTask QueueEventAsync(CloudEvent cloudEvent);
        ValueTask<CloudEvent> DequeueAsync(CancellationToken cancellationToken);
        int GetLength();
    }

    public class EventForwarderQueue : IEventForwarderQueue
    {
        private readonly Channel<CloudEvent> _queue;
        public EventForwarderQueue(int capacity = 1000)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<CloudEvent>(options);
        }

        public async ValueTask QueueEventAsync(CloudEvent cloudEvent)
        {
            await _queue.Writer.WriteAsync(cloudEvent);
        }

        public async ValueTask<CloudEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }

        public int GetLength()
        {
            return (int)_queue.GetType()
                                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                                .First(x => x.Name == "ItemsCountForDebugger")
                                .GetValue(_queue);
        }
    }

    public class EventForwarder : BackgroundService
    {
        private readonly ILogger Logger;
        public IEventForwarderQueue Queue;
        public HttpClient client;
        private readonly string _apiKey;
        public EventForwarder(IEventForwarderQueue queue, 
                              ILogger logger,
                              IHttpClientFactory clientFactory,
                              IConfiguration configuration)
        {
            Queue = queue;
            Logger = logger;
            client = clientFactory.CreateClient("amplitude");
            _apiKey = configuration.GetValue<string>("AMPLITUDE_API_KEY");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        // TODO: Rewrite to batch events pulled off the queue
        private async Task DoWork(CancellationToken stoppingToken)
        {
            var serializationOptions = new JsonSerializerOptions {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                IgnoreNullValues = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            serializationOptions.Converters.Add(new CloudEventAsPayloadConverter());
            
            while (!stoppingToken.IsCancellationRequested)
            {
                CloudEvent cloudEvent = await Queue.DequeueAsync(stoppingToken);
                try
                {
                    var messages = new List<CloudEvent>();
                    messages.Add(cloudEvent);
                    AmplitudeEventMessage evt = new AmplitudeEventMessage{
                        ApiKey = _apiKey,
                        Events = messages
                    };

                    var resp = await client.PostAsJsonAsync("", evt, serializationOptions);
                    resp.EnsureSuccessStatusCode();
                    Logger.Debug("Sent event {0} to amplitude.", cloudEvent.Id);
                
                }
                catch (HttpRequestException httpEx)
                {
                    Logger.Error(httpEx, httpEx.Message);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, 
                        "Error forwarding Event: {0}", cloudEvent.Id);
                }
            }
        }




        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}