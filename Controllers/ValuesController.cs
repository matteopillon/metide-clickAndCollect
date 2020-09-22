using ClickAndCollect.Auth;
using ClickAndCollect.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [JwtAuthentication]
        [Authorize]
        public IEnumerable<string> Get()
        {
            if(!this.User.Identity.IsAuthenticated)
            {
                throw new ApplicationException();
            }
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
