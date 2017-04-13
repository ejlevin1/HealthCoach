using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HealthCoach.App.Config;
using HealthCoach.Controllers;
using Serilog;
using Loggly.Config;
using Loggly;
using HealthCoach.Middleware;
using HealthCoach.App;

namespace HealthCoach
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            ConfigureLogging(env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private void ConfigureLogging(IHostingEnvironment env)
        {
            var config = LogglyConfig.Instance;
            config.CustomerToken = "bd919fb8-1661-434c-bab3-4751fab77e9a";
            config.ApplicationName = $"HealthCoach-{env.EnvironmentName}";

            config.Transport.EndpointHostname = "logs-01.loggly.com";
            config.Transport.EndpointPort = 443;
            config.Transport.LogTransport = LogTransport.Https;

            var ct = new ApplicationNameTag();
            ct.Formatter = "application-{0}";
            config.TagConfig.Tags.Add(ct);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Loggly(Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<FacebookOptions>(new FacebookOptions() {
                   AppSecret = "6259b3e435b7971f92f2af0dc38792ca",
                   AppToken = "EAAPjbVpgI9cBALWZBRS0xVkxhTZCNZBBR3eS388MU5zrmx9uQiLPCeohEeo6VglNzKDbJHHmMbW0LELGvnBLJTbefmzd4DREz1SF4QOcqnoofk8FPzCELV5Iop8ZACR5G5L7P9LzrG4nGe9PEx3KYnwZBBV5OZBhPiqS1tlRZA3tAZDZD",
                   ShouldVerifySignature = false,
                   VerifyToken = "my_verify_token"
            });
            services.AddSingleton<DialogManager>();

            Azure.RegisterServices(Configuration, services).Wait();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseMiddleware<SerilogMiddleware>();
            app.UseMvcWithDefaultRoute();
        }
    }
}
