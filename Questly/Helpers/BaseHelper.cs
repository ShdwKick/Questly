
using System.Text.RegularExpressions;

namespace Questly.Helpers;

public class BaseHelper
{
    public static int GenerateCode() => new Random().Next(100000, 999999);
        
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        var regex = new Regex(pattern);
        
        return regex.IsMatch(email);
    }
}