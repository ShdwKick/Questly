using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Questly.Helpers;
using DataModels;
using DataModels.Helpers;
using System.Reflection;

namespace QuestlyTests.Questly.HelpersTests
{
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
            public string? GetSalt() => "somesalt";
        }

        private HttpContextAccessor CreateHttpContextAccessorWithAuth(string? headerValue)
        {
            var context = new DefaultHttpContext();
            if (headerValue != null)
                context.Request.Headers["Authorization"] = headerValue;
            var accessor = new HttpContextAccessor { HttpContext = context };
            return accessor;
        }

        private TokenHelper CreateTokenHelperWithAccessor(HttpContextAccessor accessor)
        {
            var config = new TestConfigurationHelper();
            var helper = new TokenHelper(config);
            var field = typeof(TokenHelper).GetField("_httpContextAccessor", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(helper, accessor);
            return helper;
        }

        [Fact]
        public void GetTokenFromHeader_ReturnsToken_WhenBearerHeaderPresent()
        {
            var accessor = CreateHttpContextAccessorWithAuth("Bearer mytoken123");
            var helper = CreateTokenHelperWithAccessor(accessor);

            var token = helper.GetTokenFromHeader();

            Assert.Equal("mytoken123", token);
        }

        [Fact]
        public void GetTokenFromHeader_ThrowsArgumentException_WhenHeaderMissing()
        {
            var accessor = CreateHttpContextAccessorWithAuth(null);
            var helper = CreateTokenHelperWithAccessor(accessor);

            Assert.Throws<ArgumentException>(() => helper.GetTokenFromHeader());
        }

        [Fact]
        public void GetTokenFromHeader_ThrowsArgumentException_WhenHeaderNotBearer()
        {
            var accessor = CreateHttpContextAccessorWithAuth("Token mytoken");
            var helper = CreateTokenHelperWithAccessor(accessor);

            Assert.Throws<ArgumentException>(() => helper.GetTokenFromHeader());
        }

        [Fact]
        public void GenerateTokens_ReturnsValidJwtAndRefreshToken()
        {
            var accessor = CreateHttpContextAccessorWithAuth(null);
            var helper = CreateTokenHelperWithAccessor(accessor);

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
            var accessor = CreateHttpContextAccessorWithAuth(null);
            var helper = CreateTokenHelperWithAccessor(accessor);

            var userId = Guid.NewGuid().ToString();
            var jti = Guid.NewGuid().ToString();

            var jwt = helper.GenerateAccessToken(userId, jti);

            var expires = jwt.ValidTo;
            Assert.InRange((expires - DateTime.UtcNow).TotalMinutes, 13, 16);
        }

        [Fact]
        public void GenerateAccessToken_SignedWithServerKey()
        {
            var accessor = CreateHttpContextAccessorWithAuth(null);
            var helper = CreateTokenHelperWithAccessor(accessor);

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
}
