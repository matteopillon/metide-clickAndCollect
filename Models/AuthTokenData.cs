using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Models
{
    public class AuthTokenData
    {
        public string JwtToken { get; set; }
        public string ExternalToken { get; set; }

        public string Data { get; set; }
    }
}