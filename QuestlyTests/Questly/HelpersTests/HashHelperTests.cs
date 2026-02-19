using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;
using Questly.DataBase;

namespace QuestlyTests.Questly.HelpersTests
{
    public class HashHelperTests
    {
        [Fact]
        public void GenerateSalt_DefaultSize_ReturnsBase64WithCorrectByteLength()
        {
            string salt = HashHelper.GenerateSalt();
            byte[] bytes = Convert.FromBase64String(salt);
            Assert.Equal(16, bytes.Length);
        }

        [Fact]
        public void GenerateSalt_CustomSize_ReturnsByteArrayWithRequestedLength()
        {
            int size = 32;
            string salt = HashHelper.GenerateSalt(size);
            byte[] bytes = Convert.FromBase64String(salt);
            Assert.Equal(size, bytes.Length);
        }

        [Fact]
        public void GenerateSalt_MultipleCalls_ProduceDifferentSalts()
        {
            var salts = new HashSet<string>();
            for (int i = 0; i < 10; i++)
            {
                salts.Add(HashHelper.GenerateSalt());
            }

            Assert.True(salts.Count > 1);
        }

        [Fact]
        public void ComputeHash_WithSalt_IsDeterministicAndHasExpectedLengthAndFormat()
        {
            string input = "password123";
            string salt = "somesalt";

            string h1 = HashHelper.ComputeHash(input, salt);
            string h2 = HashHelper.ComputeHash(input, salt);

            Assert.Equal(h1, h2);
            Assert.Equal(64, h1.Length); // SHA256 hex length
            Assert.Matches(new Regex("^[0-9a-f]{64}$"), h1);
        }

        [Fact]
        public void ComputeHash_WithDifferentSalt_ProducesDifferentHashes()
        {
            string input = "password123";
            string h1 = HashHelper.ComputeHash(input, "salt1");
            string h2 = HashHelper.ComputeHash(input, "salt2");

            Assert.NotEqual(h1, h2);
        }

        [Fact]
        public void ComputeHash_WithoutSalt_IsDeterministic()
        {
            string input = "another-password";
            string h1 = HashHelper.ComputeHash(input);
            string h2 = HashHelper.ComputeHash(input);

            Assert.Equal(h1, h2);
            Assert.Equal(64, h1.Length);
        }

        [Fact]
        public void ComputeHash_NullOrWhitespaceInput_WithoutSalt_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(null!));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(""));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash("   "));
        }

        [Fact]
        public void ComputeHash_NullOrWhitespaceInput_WithSalt_ThrowsArgumentNullException()
        {
            string salt = "somesalt";
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(null!, salt));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash("", salt));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash("   ", salt));
        }

        [Fact]
        public void ComputeHash_NullOrWhitespaceSalt_WithInput_ThrowsArgumentNullException()
        {
            string input = "password123";
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, null!));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, ""));
            Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, "   "));
        }
    }
}
