using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Formatting.Json;


namespace amplitude.Services
{
    public static class SerilogLogger
    {
        public static IServiceCollection UseSerilogLogger(this IServiceCollection services, LoggerConfiguration loggerConfig)
        {
            Log.Logger = loggerConfig.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services.AddSingleton<ILogger>(Log.Logger);
        }

        public static LoggerConfiguration GetLoggerConfiguration(IConfiguration Configuration = null)
        {
             var logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationIdHeader("X-Request-Id");

            if (Configuration != null)
            {
                if (Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production")
                {
                    logConfig.WriteTo.Console(new JsonFormatter(), LogEventLevel.Information);
                }
                else
                {
                    logConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Debug);
                    logConfig.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug);
                    logConfig.WriteTo.Debug(LogEventLevel.Debug);
                }
            }
            else
            {
                logConfig.WriteTo.Debug(LogEventLevel.Debug);
            }
            return logConfig;
        }

        public static ILogger GetSerilogLogger(this IServiceCollection services)
        {
            IServiceProvider _provider = services.BuildServiceProvider();
            return _provider.GetRequiredService<ILogger>();
        }

        public static void LogError(this ILogger logger, string message)
        {
            logger.Error(message);
        }

        public static void LogDebug(this ILogger logger, string message)
        {
            logger.Debug(message);
        }

        public static void LogInformation(this ILogger logger, string message)
        {
            logger.Information(message);
        }
    }
} 