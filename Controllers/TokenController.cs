using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.Logs;
using ClickAndCollect.Models;
using System.Net;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class TokenController : ApiController
    {
        private readonly ITokenManager tokenManager;
        private readonly ILogger logger;
        private readonly ITokenStore tokenStore;

        public TokenController(ITokenManager tokenManager, ITokenStore tokenStore, ILogger logger)
        {
            this.tokenManager = tokenManager;
            this.logger = logger;
            this.tokenStore = tokenStore;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Authenticate(string username, string password)
        {
            logger.Info(nameof(Authenticate));
            if (CheckUser(username, password))
            {
                string token = tokenManager.GenerateToken(username);
                AuthTokenData tokenData = new AuthTokenData() { Data = "test", ExternalToken = username, JwtToken = token };
                tokenStore.SaveAuthToken(tokenData);
                return Ok(token);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        private bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }
    }
}
