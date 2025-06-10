using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Questly.Helpers;

public class TokenHelper : BaseHelper
{
    public static string GetTokenFromHeader()
    {
        using var scope = _serviceProvider.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        if (httpContextAccessor == null)
            throw new ArgumentException("ERROR_OCCURRED");

        var httpContext = httpContextAccessor.HttpContext;
        string authorizationHeader = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new ArgumentException("INVALID_AUTHORIZATION_HEADER_PROBLEM");
        }

        return authorizationHeader.Substring("Bearer ".Length).Trim();
    }
    
    public static JwtSecurityToken GenerateNewToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.GetServerKey()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var newToken = new JwtSecurityToken(
            issuer: ConfigurationHelper.GetIssuer(),
            audience: ConfigurationHelper.GetAudience(),
            claims: claims,
            expires: DateTime.Now.AddMinutes(480),
            signingCredentials: credentials
        );
        return newToken;
    }
}