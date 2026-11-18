namespace Questly.Authorization;

/// <summary>
/// Имена политик авторизации. Строки вынесены в константы, чтобы опечатка
/// в атрибуте не превращалась в молча открытую ручку.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Требует, чтобы запрос шёл от действующего администратора.
    /// </summary>
    public const string Admin = "Admin";
}
