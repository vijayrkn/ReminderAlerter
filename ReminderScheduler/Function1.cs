using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReminderScheduler
{
    public class ReminderScheduler
    {
        private IConfiguration _configuration;

        public ReminderScheduler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("ReminderScheduler")]
        public void Run([TimerTrigger("* * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string api = _configuration["API"];
            if (!string.IsNullOrEmpty(api))
            {
                HttpClient client = new HttpClient();
                client.GetAsync(@"{api}/Reminder");
            }
        }
    }
}
