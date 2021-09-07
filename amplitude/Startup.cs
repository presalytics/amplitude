using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using amplitude.Services;
using CloudNative.CloudEvents.SystemTextJson;
using amplitude.Serialization;

namespace amplitude
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var loggerConfig = SerilogLogger.GetLoggerConfiguration();
            services.UseSerilogLogger(loggerConfig);
            services.AddSingleton<IEventForwarderQueue, EventForwarderQueue>();
            services.AddHostedService<EventForwarder>();
            services.AddHttpClient("amplitude", c => {
                c.BaseAddress = new Uri(string.Format("http://{0}", Configuration.GetValue<string>("AMPLITUDE_HOST", "api.amplitude.com/2/httpapi")));
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                );
                c.DefaultRequestHeaders.Add("User-Agent", "presaltyics-amplitude-service");
            });

            services.AddControllers(opts =>
            {
                opts.InputFormatters.Insert(0, new CloudEventJsonInputFormatter(new JsonEventFormatter()));
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
