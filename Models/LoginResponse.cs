using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Models
{
    public class LoginResponse
    {
        public ExternalUser User { get; set; }
        public string Token { get; set; }
    }
}