using System.Security.Cryptography;
using System.Text;
using Questly.Helpers;

namespace Questly.DataBase;

public class HashHelper : BaseHelper
{
    public static string GenerateSalt(int size = 16)
    {
        var saltBytes = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
    
    public static string ComputeHash(string input, string salt)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentNullException(nameof(salt));
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input + salt);
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    public static string ComputeHash(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentNullException(nameof(input));
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}