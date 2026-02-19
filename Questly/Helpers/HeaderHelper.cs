using System.IdentityModel.Tokens.Jwt;
using DataModels.Helpers;

namespace Questly.Helpers;

public class HeaderHelper : IHeaderHelper
{
    private readonly ITokenHelper _tokenHelper;

    public HeaderHelper(ITokenHelper tokenHelper)
    {
        _tokenHelper = tokenHelper;
    }

    public Guid GetUserIdFromHeader()
    {
        var token = _tokenHelper.GetTokenFromHeader();
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