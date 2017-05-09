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
using Quartz.Impl;
using Quartz;
using System.Collections.Specialized;

namespace HealthCoach
{
    public class Startup
    {
        private IScheduler _Scheduler = null;

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
            if(false)//env.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
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
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            try
            {
                throw new Exception("Random exception");
                _Scheduler = Scheduling.Create().Result;
                services.AddSingleton<IScheduler>(_Scheduler);
            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex, "Failed to startup scheduling.");
            }

            services.AddSingleton<FacebookOptions>(new FacebookOptions() {
                   AppSecret = "6259b3e435b7971f92f2af0dc38792ca",
                   AppToken = "EAAPjbVpgI9cBALWZBRS0xVkxhTZCNZBBR3eS388MU5zrmx9uQiLPCeohEeo6VglNzKDbJHHmMbW0LELGvnBLJTbefmzd4DREz1SF4QOcqnoofk8FPzCELV5Iop8ZACR5G5L7P9LzrG4nGe9PEx3KYnwZBBV5OZBhPiqS1tlRZA3tAZDZD",
                   ShouldVerifySignature = false,
                   VerifyToken = "my_verify_token"
            });
            services.AddSingleton<DialogManager>();

            //string eventhubHostFormat = "amqps://{0}:{1}@{2}.servicebus.windows.net";
            //var address = string.Format(eventhubHostFormat, "RootManageSharedAccessKey", Uri.EscapeUriString("i4O7XCN0bK7wjbyQJo1H30ZfTb0RVPejysh1pmMu/Zw="), "healthcoach");
            //services.AddSingleton<IEventingClient>(new AzureEventingClient(address));

            Azure.RegisterServices(Configuration, services).Wait();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseMiddleware<SerilogMiddleware>();
            app.UseMvcWithDefaultRoute();

            try
            {
                lifetime.ApplicationStopping.Register(OnShutdown);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Failed to register shutdown event.");
            }
        }

        private void OnShutdown()
        {
            if (_Scheduler != null)
                _Scheduler.Shutdown().Wait();
        }
    }
}
