using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.Extensions.DependencyInjection;
using Questly.Repositories;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace Questly.Helpers;

public class UserHelper : BaseHelper
{
    public static Guid GetUserIdFromHeader()
    {
        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            
        var token = TokenHelper.GetTokenFromHeader();
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("AUTH_TOKEN_MISSING_PROBLEM", nameof(token));

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwtToken == null)
            throw new ArgumentException("INVALID_TOKEN_PROBLEM");

        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        if (claim == null)
            throw new ArgumentException("INVALID_TOKEN_CLAIMS_PROBLEM");

        if (!Guid.TryParse(claim.Value, out var userId))
            throw new ArgumentException("AUTH_TOKEN_CLAIM_INVALID");
        
        return userId;
    }
}