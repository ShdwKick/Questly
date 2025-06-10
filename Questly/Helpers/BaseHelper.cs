
namespace Questly.Helpers
{
    public class BaseHelper
    {
        protected static IServiceProvider _serviceProvider;

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        // public static int GenerateCode() => new Random().Next(100000, 999999);
        //
        // public static bool IsValidEmail(string email)
        // {
        //     if (string.IsNullOrWhiteSpace(email))
        //         return false;
        //
        //     string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        //     Regex regex = new Regex(pattern);
        //
        //     return regex.IsMatch(email);
        // }
    }
}