using Microsoft.AspNetCore.Authorization;

namespace Questly.Authorization;

/// <summary>
/// Требование «пользователь — администратор». Проверяется
/// <see cref="AdminAuthorizationHandler"/> по данным из базы.
/// </summary>
public class AdminRequirement : IAuthorizationRequirement;
