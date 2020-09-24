using ClickAndCollect.Auth;
using ClickAndCollect.Logs;
using ClickAndCollect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Proxies
{
    public class TestServiceProxy : IServiceProxy
    {

        private const string TestSecret = "853BDA13BAC62B7224AF452F5658FBE01D0FDEBD0B1BD7638F289B37D3C06EAA";
        private readonly List<ExternalUser> users = new List<ExternalUser>();
        private readonly Dictionary<string, ExternalUser> logins = new Dictionary<string, ExternalUser>();
        private readonly ILogger logger;
        private readonly JwtTokenManager tokenManager;

        public TestServiceProxy(ILogger logger)
        {
            this.logger = logger;
            this.tokenManager = new JwtTokenManager(logger, TestSecret);

            users.Add(new ExternalUser() { Name = "Name", Surname = "Surname", Username = "testuser" });
            users.Add(new ExternalUser() { Name = "Metide", Surname = "Metide", Username = "metide" });
            users.Add(new ExternalUser() { Name = "Carli", Surname = "Carli", Username = "carli" });

        }

        public ExternalUser GetUser(string username)
        {
            return users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
        }

        public ExternalLogin LoginUser(string username, string password)
        {

            var user = GetUser(username);
            if (user != null)
            {
                var token = tokenManager.GenerateToken(username);
                logins.Add(token, user);
                return new ExternalLogin() { JwtToken = token };
            }
            else
            {
                return null;
            }

        }
    }
}