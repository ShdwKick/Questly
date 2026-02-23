using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Questly.Helpers;
using DataModels;
using DataModels.Helpers;
using Moq;

namespace QuestlyTests.Questly.HelpersTests;

public class TokenHelperTests
{
    private class TestConfigurationHelper : IConfigurationHelper
    {
        public string GetServerKey() => "super-secret-key-which-is-long-enough";
        public string GetIssuer() => "test-issuer";
        public string GetAudience() => "test-audience";
        public string GetRabbitHostName() => "rabbit-host";
        public string GetRabbitUserName() => "rabbit-user";
        public string GetRabbitPassword() => "rabbit-pass";
        public string GetSalt() => "somesalt";
    }

    private static TokenHelper CreateTokenHelperWithAuthHeader(string? headerValue)
    {
        var httpContext = new DefaultHttpContext();
        if (headerValue != null)
            httpContext.Request.Headers.Authorization = headerValue;
        
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        
        var config = new TestConfigurationHelper();
        return new TokenHelper(config, httpContextAccessorMock.Object);
    }

    [Fact]
    public void GetTokenFromHeader_ReturnsToken_WhenBearerHeaderPresent()
    {
        var helper = CreateTokenHelperWithAuthHeader("Bearer mytoken123");

        var token = helper.GetTokenFromHeader();

        Assert.Equal("mytoken123", token);
    }

    [Fact]
    public void GetTokenFromHeader_ThrowsArgumentException_WhenHeaderMissing()
    {
        var helper = CreateTokenHelperWithAuthHeader(null);

        var exception = Assert.Throws<ArgumentException>(() => helper.GetTokenFromHeader());
        Assert.Equal("INVALID_AUTHORIZATION_HEADER_PROBLEM", exception.Message);
    }

    [Fact]
    public void GetTokenFromHeader_ThrowsArgumentException_WhenHeaderNotBearer()
    {
        var helper = CreateTokenHelperWithAuthHeader("Token mytoken");

        var exception = Assert.Throws<ArgumentException>(() => helper.GetTokenFromHeader());
        Assert.Equal("INVALID_AUTHORIZATION_HEADER_PROBLEM", exception.Message);
    }

    [Fact]
    public void GetTokenFromHeader_ThrowsArgumentException_WhenHeaderEmpty()
    {
        var helper = CreateTokenHelperWithAuthHeader("Bearer ");

        var exception = Assert.Throws<ArgumentException>(() => helper.GetTokenFromHeader());
        Assert.Equal("INVALID_AUTHORIZATION_HEADER_PROBLEM", exception.Message);
    }

    [Fact]
    public void GenerateTokens_ReturnsValidJwtAndRefreshToken()
    {
        var helper = CreateTokenHelperWithAuthHeader(null);

        var user = new User { Id = Guid.NewGuid() };
        var jti = Guid.NewGuid().ToString();

        var pair = helper.GenerateTokens(user, jti);

        Assert.False(string.IsNullOrWhiteSpace(pair.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(pair.RefreshToken));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(pair.AccessToken);
        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Equal("test-audience", jwt.Audiences.FirstOrDefault());

        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Jti && c.Value == jti);

        var bytes = Convert.FromBase64String(pair.RefreshToken);
        Assert.Equal(32, bytes.Length);
    }

    [Fact]
    public void GenerateAccessToken_ProducesTokenWithExpiryAbout15Minutes()
    {
        var helper = CreateTokenHelperWithAuthHeader(null);

        var userId = Guid.NewGuid().ToString();
        var jti = Guid.NewGuid().ToString();

        var jwt = helper.GenerateAccessToken(userId, jti);

        var expires = jwt.ValidTo;
        Assert.InRange((expires - DateTime.UtcNow).TotalMinutes, 13, 16);
    }

    [Fact]
    public void GenerateAccessToken_SignedWithServerKey()
    {
        var helper = CreateTokenHelperWithAuthHeader(null);

        var userId = Guid.NewGuid().ToString();
        var jti = Guid.NewGuid().ToString();

        var jwt = helper.GenerateAccessToken(userId, jti);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

        var valParams = new TokenValidationParameters
        {
            ValidIssuer = "test-issuer",
            ValidAudience = "test-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new TestConfigurationHelper().GetServerKey())),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(tokenString, valParams, out var validated);
        Assert.NotNull(validated);
    }
}