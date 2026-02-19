using System.IdentityModel.Tokens.Jwt;

namespace DataModels.Helpers;

public interface ITokenHelper
{
    string GetTokenFromHeader();
    TokenPair GenerateTokens(User user, string jti);
    JwtSecurityToken GenerateAccessToken(string userId, string jti);
}