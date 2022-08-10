using System.Security.Claims; 

namespace WebApp.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userName);
        string ValidateToken(string token); 
        ClaimsPrincipal GetPrincipal(string token); 
        string GenerateRefreshToken();

    }
}