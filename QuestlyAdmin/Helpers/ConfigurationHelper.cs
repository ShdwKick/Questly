namespace QuestlyAdmin.Helpers
{
    public class ConfigurationHelper
    {
        public static string GetSalt()
        {
            return Environment.GetEnvironmentVariable("HASH_SALT");
        }

        public static string GetServerKey()
        {
            return Environment.GetEnvironmentVariable("SERVER_KEY");
        }

        public static string GetBaseUrl()
        {
            return Environment.GetEnvironmentVariable("BASE_URL");
        }

        public static string GetIssuer()
        {
            return Environment.GetEnvironmentVariable("ISSUER");
        }

        public static string GetAudience()
        {
            return Environment.GetEnvironmentVariable("AUDIENCE");
        }
        
        public static string GetRabbitHostName()
        {
            return Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME");
        }
        public static string GetRabbitUserName()
        {
            return Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        }
        public static string GetRabbitPassword()
        {
            return Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
        }
        
    }
}