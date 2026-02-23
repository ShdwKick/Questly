namespace DataModels.Results;

/// <summary>
/// Вспомогательный класс для пагинации
/// </summary>
/// <typeparam name="T">Тип данных</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Элементы текущей страницы
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    public int PageSize { get; set; }
}