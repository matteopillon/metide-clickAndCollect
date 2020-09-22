using ClickAndCollect.Models;
using LazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Data
{
    public class CacheTokenStore : ITokenStore
    {
        private readonly ITokenStore tokenStore;
        private LazyCache.IAppCache cache;

        public CacheTokenStore(ITokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
            cache = new CachingService();
        }

        public bool DeleteAuthToken(string jwtAuthToken)
        {
            if (tokenStore.DeleteAuthToken(jwtAuthToken))
            {
                cache.Remove(jwtAuthToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        public AuthTokenData GetAuthToken(string jwtAuthToken)
        {
            var result = cache.Get<AuthTokenData>(jwtAuthToken);
            if(result == null)
            {
                result = tokenStore.GetAuthToken(jwtAuthToken);
                if(result != null)
                {
                    cache.Add<AuthTokenData>(jwtAuthToken, result);
                }
            }
            return result;
        }

        public IEnumerable<AuthTokenData> GetAuthTokens()
        {
            throw new NotImplementedException();
        }

        public bool SaveAuthToken(AuthTokenData jwtAuthTokenData)
        {
            if (tokenStore.SaveAuthToken(jwtAuthTokenData))
            {
                cache.Remove(jwtAuthTokenData.JwtToken);
                cache.Add<AuthTokenData>(jwtAuthTokenData.JwtToken,jwtAuthTokenData);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}