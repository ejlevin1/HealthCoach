using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HealthCoach.App.Config;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HealthCoach.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IAzureFileWriter Writer;

        public ValuesController(IAzureFileWriter writer)
        {
            Writer = writer;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]object value)
        {
            Writer.Write(value.ToString());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
