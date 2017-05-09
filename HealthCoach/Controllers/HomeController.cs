using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HealthCoach.Core.Eventing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HealthCoach.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private IEventingClient _eventingClient;

        public HomeController(ILogger<HomeController> logger, IEventingClient client)
        {
            _logger = logger;
            _eventingClient = client;
        }

        [Route("")]
        public string Index()
        {
            _logger.LogInformation("/ Request Received");
            _eventingClient.Publish(new { Name = "Eric" });

            return string.Format("The server is online as of {0}", DateTime.UtcNow);
        }
    }
}
