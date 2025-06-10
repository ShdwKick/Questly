namespace Questly.Services
{
    public interface IRabbitService
    {
        Task InitializeServiceAsync();
        Task PublishMessageAsync(string messageType, object message);
    }
}