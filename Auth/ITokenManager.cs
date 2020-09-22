
using ClickAndCollect.Models;
using System.Security.Claims;

namespace ClickAndCollect.Auth
{
    public interface ITokenManager
    {
        string GenerateToken(string username);

        ClaimsPrincipal GetPrincipal(string token);
    }
}