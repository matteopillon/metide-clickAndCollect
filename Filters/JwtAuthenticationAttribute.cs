﻿using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace ClickAndCollect.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        private ITokenManager tokenManager = null;
        private bool checkTokenInStore;

        public JwtAuthenticationAttribute(bool checkTokenInStore = true)
        {
            this.checkTokenInStore = checkTokenInStore;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }
            var dependencyScope = context.Request.GetDependencyScope();
            tokenManager = dependencyScope.GetService(typeof(ITokenManager)) as ITokenManager;
         
            if (tokenManager == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token Manager", request);
                return;
            }

            var tokenStore = dependencyScope.GetService(typeof(ITokenStore)) as ITokenStore;
            if (tokenStore == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token Store", request);
                return;
            }

            //Check if jwt token exists on store (only if the user is logged)
            if (checkTokenInStore)
            {
                var loginToken = tokenStore.GetAuthToken(authorization.Parameter);
                if (loginToken == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Token not found", request);
                    return;
                }
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }

        private bool ValidateToken(string token, out string username)
        {
            username = null;

            var simplePrinciple = tokenManager.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            if (ValidateToken(token, out var username))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, token)
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}