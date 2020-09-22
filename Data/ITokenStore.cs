using ClickAndCollect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Data
{
    public interface ITokenStore
    {
        IEnumerable<AuthTokenData> GetAuthTokens();

        AuthTokenData GetAuthToken(string jwtAuthToken);

        bool SaveAuthToken(AuthTokenData jwtAuthTokenData);

        bool DeleteAuthToken(string jwtAuthToken);
    }
}