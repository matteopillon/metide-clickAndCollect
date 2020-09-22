using ClickAndCollect.Auth;
using ClickAndCollect.Logs;
using System.Net;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class TokenController : ApiController
    {
        private readonly ITokenManager tokenManager;
        private readonly ILogger logger;

        public TokenController(ITokenManager tokenManager,ILogger logger)
        {
            this.tokenManager = tokenManager;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Authenticate(string username, string password)
        {
            logger.Info(nameof(Authenticate));
            if (CheckUser(username, password))
            {
                return Ok(tokenManager.GenerateToken(username));
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
