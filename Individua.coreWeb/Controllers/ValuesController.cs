using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Individua.coreWeb.Infs;
using Microsoft.AspNetCore.Mvc;

namespace Individua.coreWeb.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IEnumerable<IDog> dogs;

        public ValuesController(IEnumerable<IDog> _dogs)
        {
            dogs = _dogs;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //return new string[] { "value1", "value2" };
            List<string> list = new List<string>();
            foreach (var dog in dogs)
            {
                list.Add($"名称：{dog.Name},品种：{dog.Breed}");
            }
            return list.ToArray();

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
