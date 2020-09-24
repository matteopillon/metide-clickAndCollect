using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.Filters;
using ClickAndCollect.Logs;
using ClickAndCollect.Models;
using ClickAndCollect.Proxies;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Web.Http;

namespace ClickAndCollect.Controllers
{
    public class TokenController : ExternaJWTBaseController
    {
        private readonly ITokenManager tokenManager;
        private readonly ILogger logger;
        private readonly ITokenStore tokenStore;
        private readonly IServiceProxy serviceProxy;

        public TokenController(ITokenManager tokenManager, ITokenStore tokenStore, IServiceProxy serviceProxy, ILogger logger) :
            base(tokenStore)
        {
            this.tokenManager = tokenManager;
            this.logger = logger;
            this.tokenStore = tokenStore;
            this.serviceProxy = serviceProxy;
        }

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Authenticate([FromBody] LoginRequest loginRequest)
        {
            try
            {
                logger.Info(nameof(Authenticate));
                if (loginRequest == null) return BadRequest();
                if (string.IsNullOrEmpty(loginRequest.Username)) return BadRequest();
                if (string.IsNullOrEmpty(loginRequest.Password)) return BadRequest();

                var tokenData = CheckUser(loginRequest);
                if (tokenData != null)
                {
                    return Ok(tokenData);
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                logger.Error(nameof(Authenticate), ex);
                return InternalServerError();
            }
        }

        private LoginResponse CheckUser(LoginRequest loginRequest)
        {
            // Login User with external service
            var externalLogin = serviceProxy.LoginUser(loginRequest.Username, loginRequest.Password);

            if (externalLogin != null)
            {
                // Create Token data
                string token = tokenManager.GenerateToken(loginRequest.Username);

                //Get user data
                var user = serviceProxy.GetUser(loginRequest.Username);

                // Save token
                AuthTokenData tokenData = new AuthTokenData() { Data = JsonConvert.SerializeObject(user), ExternalToken = externalLogin.JwtToken, JwtToken = token };
                tokenStore.SaveAuthToken(tokenData);
                return new LoginResponse() { Token = tokenData.JwtToken, User = user };
            }
            else
            {
                return null;
            }
        }


        [JwtAuthentication]
        [Authorize]
        [HttpDelete]
        public IHttpActionResult Logout()
        {
            try
            {
                if (this.TokenData == null) return BadRequest();
                if (this.TokenData.JwtToken == null) return this.InternalServerError();
                if (tokenStore.DeleteAuthToken(this.TokenData.JwtToken))
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(Authenticate), ex);
                return InternalServerError();
            }

        }
    }
}
