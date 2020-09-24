using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.Filters;
using ClickAndCollect.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class UsersController : ExternaJWTBaseController
    {
       
        public UsersController(ITokenStore tokenStore):base(tokenStore)
        {
            
        }

        // GET api/values
        [JwtAuthentication]
        [Authorize]
        public ExternalUser Get()
        {
            return JsonConvert.DeserializeObject<ExternalUser>( this.TokenData?.Data);
        }

    }
}
