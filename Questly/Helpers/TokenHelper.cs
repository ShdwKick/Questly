using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DataModels;
using DataModels.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Questly.Helpers;

public class TokenHelper : ITokenHelper
{
    private readonly IConfigurationHelper _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenHelper(IConfigurationHelper configuration)
    {
        _configuration = configuration;
    }
    public string GetTokenFromHeader()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        string authorizationHeader = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new ArgumentException("INVALID_AUTHORIZATION_HEADER_PROBLEM");
        }

        return authorizationHeader.Substring("Bearer ".Length).Trim();
    }
    
    public TokenPair GenerateTokens(User user, string jti)
    {
        var accessToken = new JwtSecurityTokenHandler().WriteToken(GenerateAccessToken(user.Id.ToString(), jti));
        var refreshToken = GenerateRefreshToken();
        return new TokenPair(accessToken, refreshToken);
    }
    
    public JwtSecurityToken GenerateAccessToken(string userId, string jti)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetServerKey()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        return new JwtSecurityToken(
            issuer: _configuration.GetIssuer(),
            audience: _configuration.GetAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

}