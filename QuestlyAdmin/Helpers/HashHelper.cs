using System.Security.Cryptography;
using System.Text;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.DataBase;

public class HashHelper : BaseHelper
{
    public static string ComputeHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + ConfigurationHelper.GetSalt());
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}