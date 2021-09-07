using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using amplitude.Services;


namespace amplitude
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try 
            {
                Log.Logger = SerilogLogger.GetLoggerConfiguration().CreateLogger();

                Log.Information("Initializing Event Service.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Stopped due to unexpected exception");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    
                    webBuilder.UseConfiguration(
                        new ConfigurationBuilder()
                            .AddEnvironmentVariables()
                            .AddCommandLine(args)
                            .Build()
                        );
                    webBuilder.UseStartup<Startup>();
                });
    }
}
