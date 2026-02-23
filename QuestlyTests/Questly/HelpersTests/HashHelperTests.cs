using System.Text.RegularExpressions;
using Questly.DataBase;

namespace QuestlyTests.Questly.HelpersTests;

public class HashHelperTests
{
    [Fact]
    public void GenerateSalt_DefaultSize_ReturnsBase64WithCorrectByteLength()
    {
        var salt = HashHelper.GenerateSalt();
        var bytes = Convert.FromBase64String(salt);
        Assert.Equal(16, bytes.Length);
    }

    [Fact]
    public void GenerateSalt_CustomSize_ReturnsByteArrayWithRequestedLength()
    {
        var size = 32;
        var salt = HashHelper.GenerateSalt(size);
        var bytes = Convert.FromBase64String(salt);
        Assert.Equal(size, bytes.Length);
    }

    [Fact]
    public void GenerateSalt_MultipleCalls_ProduceDifferentSalts()
    {
        var salts = new HashSet<string>();
        for (var i = 0; i < 10; i++)
        {
            salts.Add(HashHelper.GenerateSalt());
        }

        Assert.True(salts.Count > 1);
    }

    [Fact]
    public void ComputeHash_WithSalt_IsDeterministicAndHasExpectedLengthAndFormat()
    {
        const string input = "password123";
        const string salt = "somesalt";

        var h1 = HashHelper.ComputeHash(input, salt);
        var h2 = HashHelper.ComputeHash(input, salt);

        Assert.Equal(h1, h2);
        Assert.Equal(64, h1.Length); // SHA256 hex length
        Assert.Matches(new Regex("^[0-9a-f]{64}$"), h1);
    }

    [Fact]
    public void ComputeHash_WithDifferentSalt_ProducesDifferentHashes()
    {
        var input = "password123";
        var h1 = HashHelper.ComputeHash(input, "salt1");
        var h2 = HashHelper.ComputeHash(input, "salt2");

        Assert.NotEqual(h1, h2);
    }

    [Fact]
    public void ComputeHash_WithoutSalt_IsDeterministic()
    {
        var input = "another-password";
        var h1 = HashHelper.ComputeHash(input);
        var h2 = HashHelper.ComputeHash(input);

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
        const string salt = "somesalt";
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(null!, salt));
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash("", salt));
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash("   ", salt));
    }

    [Fact]
    public void ComputeHash_NullOrWhitespaceSalt_WithInput_ThrowsArgumentNullException()
    {
        var input = "password123";
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, null!));
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, ""));
        Assert.Throws<ArgumentNullException>(() => HashHelper.ComputeHash(input, "   "));
    }
}