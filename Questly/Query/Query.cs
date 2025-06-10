using HotChocolate;

namespace Questly.Queries
{
    public class Query
    {
        [GraphQLDescription("Получить серверное время")]
        public DateTime GetServerCurrentDateTime()
        {
            return DateTime.Now;
        }

        [GraphQLDescription("Получить серверное время по UTC")]
        public DateTime GetServerCurrentUTCDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}