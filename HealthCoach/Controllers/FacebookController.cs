using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using HealthCoach.App.Config;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HealthCoach.App.FacebookEntities;
using RestSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HealthCoach.Controllers
{
    [Route("api/[controller]")]
    public class FacebookController : Controller
    {
        private FacebookOptions _options;
        private IAzureFileWriter _writer;
        private readonly ILogger _logger;
        private Dictionary<string, string> _photosAwaitingCategorization = new Dictionary<string, string>();

        public FacebookController(FacebookOptions options, IAzureFileWriter writer, ILogger<FacebookController> logger)
        {
            _options = options;
            _writer = writer;
            _logger = logger;
        }

        [HttpGet]
        public string Get([FromQuery(Name = "hub.mode")] string hub_mode,
            [FromQuery(Name = "hub.challenge")] string hub_challenge,
            [FromQuery(Name = "hub.verify_token")] string hub_verify_token)
        {
            if (_options.VerifyToken == hub_verify_token)
            {
                _logger.LogInformation("Get received. Token OK : {0}", hub_verify_token);
                return hub_challenge;
            }
            else
            {
                _logger.LogError("Error. Token did not match. Got : {0}, Expected : {1}", hub_verify_token, _options.VerifyToken);
                return "error. no match";
            }
        }

        [HttpPost]
        public void Post([FromBody]JToken body)
        {
            try
            {
                _writer.Write(body.ToString());

                Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(body.ToString());

                if(!IsNullOrEmpty(obj.entry))
                {
                    var entry = obj.entry[0];
                    if(!IsNullOrEmpty(entry.messaging))
                    {
                        var messaging = entry.messaging[0];

                        if(messaging.message != null)
                        {
                            foreach(var attachment in messaging.message.attachments)
                            {
                                if(attachment.type == "image")
                                {
                                    _photosAwaitingCategorization.Add(messaging.sender.id, attachment.payload.url);
                                    
                                    SendMessageToUser(messaging.sender.id, "Is that an image of a meal, or your stool?");
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Unknown error occurred in WebHook!", ex);
            }
        }

        [Route("test")]
        public void TestRequest()
        {
            SendMessageToUser("1663365037012180", "Is that an image of a meal, or your stool?");
        }

        private void SendMessageToUser(string user, string msg)
        {
            //Kick off response
            var request = new RestRequest("v2.6/me/messages", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("access_token", _options.AppToken);
            request.AddJsonBody(new
            {
                recipient = new { id = user },
                message = new { text = msg },
            });

            var client = new RestClient("https://graph.facebook.com");
            var handle = client.PostAsync(request, (response, requestHandle) =>
            {
                _logger.LogInformation("Completed FB request");
            });
        }

        public static bool IsNullOrEmpty(Array array)
        {
            return (array == null || array.Length == 0);
        }
    }



    public class FacebookOptions
    {
        public string VerifyToken { get; set; }
        public bool ShouldVerifySignature { get; set; }
        public string AppSecret { get; set; }
        public string AppToken { get; set; }
    }
}
