using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Example.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedProtectedConfigurationLib;

namespace Example.Bar.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly JsonFileProtector _jsonProtector;
        private readonly ConnectionStrings _connectionStrings;

        public ValuesController(JsonFileProtector jsonProtector, IOptions<ConnectionStrings> connectionStrings)
        {
            _jsonProtector = jsonProtector;
            _connectionStrings = connectionStrings.Value;
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            //_jsonProtector.ProtectFile(@"c:\work\gitreps\shared-encrypted-config\src\SharedConfiguration\appsettings.shared.encrypted.json");
            //_jsonProtector.UnprotectFile(@"c:\work\gitreps\shared-encrypted-config\src\SharedConfiguration\appsettings.shared.encrypted2.json");
            return Json(_connectionStrings);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
