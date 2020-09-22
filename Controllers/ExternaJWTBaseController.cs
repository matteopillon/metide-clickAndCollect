using ClickAndCollect.Data;
using ClickAndCollect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class ExternaJWTBaseController : ApiController
    {
        private readonly ITokenStore tokenStore;

        public ExternaJWTBaseController(ITokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
        }

        public AuthTokenData TokenData
        {
            get
            {
                var claimPrincipal = this.User as ClaimsPrincipal;
                if (claimPrincipal != null)
                {
                    var token = claimPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    if( !string.IsNullOrEmpty(token)) return tokenStore.GetAuthToken(token);
                }
                return null;
            }
        }
    }
}
