using Questly.Helpers;

namespace QuestlyTests.Questly.HelpersTests
{
    public class BaseHelpersTests
    {
        [Fact]
        public void GenerateCode_ReturnsSixDigitNumber()
        {
            int code = BaseHelper.GenerateCode();
            Assert.InRange(code, 100000, 999999);
        }

        [Fact]
        public void GenerateCode_MultipleCalls_ProduceAtLeastOneDifferent()
        {
            var codes = new HashSet<int>();
            for (int i = 0; i < 10; i++)
            {
                codes.Add(BaseHelper.GenerateCode());
            }

            Assert.True(codes.Count > 1);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("user.name+tag@sub.example.co")]
        [InlineData("user_name-123@domain.org")]
        [InlineData("a@b.co")]
        public void IsValidEmail_ReturnsTrue_ForValidEmails(string email)
        {
            Assert.True(BaseHelper.IsValidEmail(email));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("plainaddress")]
        [InlineData("user@")]
        [InlineData("@domain.com")]
        [InlineData("user@domain")]
        [InlineData("user@.com")]
        [InlineData("user@domain.c")]
        public void IsValidEmail_ReturnsFalse_ForInvalidEmails(string email)
        {
            Assert.False(BaseHelper.IsValidEmail(email));
        }
    }
}