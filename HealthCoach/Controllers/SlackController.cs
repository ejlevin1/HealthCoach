using HealthCoach.App.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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

        public SlackController(IAzureFileWriter writer, ILogger<SlackController> logger)
        {
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

                var msg = JsonConvert.DeserializeObject<App.SlackEntities.Message>(body.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Unknown error occurred in WebHook!", ex);
            }

            return string.Empty;
        }
    }
}
