using Core.Infrastructure.Application;
using Core.Infrastructure.Logging;
using Core.Infrastructure.Tracing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile(Path.Combine("config", "stagesettings.json"), true)
                .AddJsonFile(Path.Combine("config", "ocelot.json"))
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var port = configuration.GetSection("Application").Get<ApplicationConfiguration>().PortNumber;
            
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s => s.AddSingleton(builder))
                .UseConfiguration(configuration)
                .ConfigureAppConfiguration(ic => 
                {
                    ic.AddJsonFile(Path.Combine("config", "stagesettings.json"), true)
                        .AddJsonFile(Path.Combine("config", "ocelot.json"));
                })
                
                .ConfigureLogging((hostingContext, loggingbuilder) =>
                {
                    loggingbuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .ConfigureLogging()
                .UseUrls($"http://localhost:7000")
                .UseKestrel()
                .ConfigureTracing(configuration)
                .UseStartup<Startup>();
            
            builder.Build().Run();
        }
    }
}