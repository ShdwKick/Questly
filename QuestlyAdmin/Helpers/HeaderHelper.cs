using System.IdentityModel.Tokens.Jwt;

namespace QuestlyAdmin.Helpers;

public class HeaderHelper : BaseHelper
{
    public static Guid GetUserIdFromHeader()
    {
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