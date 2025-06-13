using System.Security.Cryptography;
using System.Text;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.Helpers;

public class HashHelper : BaseHelper
{
    public static string GenerateSalt(int size = 16)
    {
        byte[] saltBytes = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
    
    public static string ComputeHash(string input, string salt)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input + salt);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    public static string ComputeHash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}