namespace DataModels.Exceptions;

/// <summary>
/// Запрос корректен, но противоречит текущему состоянию данных:
/// например, город с таким названием уже есть. Фильтр
/// <c>HttpResponseExceptionFilter</c> превращает это в 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
