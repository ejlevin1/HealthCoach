using HealthCoach.App;
using HealthCoach.App.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.Controllers
{
    [Route("api/[controller]")]
    public class SlackController : Controller
    {
        private readonly ILogger _logger;
        private IAzureFileWriter _writer;
        private DialogManager _dialogManager = null;

        public SlackController(DialogManager dialogManager, IAzureFileWriter writer, ILogger<SlackController> logger)
        {
            _dialogManager = dialogManager;
            _logger = logger;
            _writer = writer;
        }

        [HttpPost]
        public string Post([FromBody]JToken body)
        {
            try
            {
                if(body["challenge"] != null)
                {
                    return body["challenge"].ToString();
                }

                var pp = JsonConvert.SerializeObject(body, Formatting.Indented,
                    new JsonConverter[] { new StringEnumConverter() });
                _writer.Write(pp);

                var root = JsonConvert.DeserializeObject<App.SlackEntities.Rootobject>(body.ToString());
                if(root.type == "event_callback" && root._event?.type == "message")
                {
                    var e = root._event;
                    _dialogManager.HandleMessage("slack", e.username, e.text, (response) => {
                        SendMessage(root.token, e.channel, response);
                    }, e.file?.permalink);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unknown error occurred in WebHook!", ex);
            }

            return string.Empty;
        }

        [HttpGet("/test")]
        public void Test()
        {
            SendMessage("G4Y2879SS", "T2fzlv8AtFsGpItmDp34WoY5", "This is a test coming from the app");

        }

        private void SendMessage(string token, string channel, string message)
        {
            var client = new RestSharp.RestClient("https://hooks.slack.com/services/");
            var request = new RestRequest("T061NBJFK/B4ZPCJ4H5/3DXp5OGHm6THUVV1gpiRov0D", Method.POST);

            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(new
            {
                text = message
            });

            var handle = client.PostAsync(request, (response, requestHandle) =>
            {
                _logger.LogInformation("Completed slack send message: " + message);
            });
        }
    }
}
