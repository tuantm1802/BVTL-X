using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebBVTLAPI.Controllers
{
    public class TestController : ApiController
    {
        [Route("api/Test/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "student1", "student2" };
        }
    }
}
