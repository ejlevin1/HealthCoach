using Quartz;
using Quartz.Impl;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.App
{
    public class Scheduling
    {
        public static async Task<IScheduler> Create()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler().Result;

            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger t2 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSchedule(CronScheduleBuilder
                                .CronSchedule("0 0 5,9,11,13,15,16,18,21 * * ?")
                                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))) // execute job daily at 11:30
                .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, t2);

            return scheduler;
        }
    }

    class HelloJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Log.Logger.Information("Started FlowXO Webhook");

            var request = new RestRequest("hooks/b/79rkq2yd", Method.GET);
            var client = new RestClient("https://flowxo.com/");
            var handle = client.PostAsync(request, (response, requestHandle) =>
            {
                Log.Logger.Information("Completed FlowXO Webhook");
            });

            return Task.FromResult<int>(1);
        }
    }
}
