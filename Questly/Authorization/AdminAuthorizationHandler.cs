using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Questly.Repositories;

namespace Questly.Authorization;

/// <summary>
/// Проверяет право администратора по базе, а не по claim в токене.
///
/// Так сделано намеренно: access-токен живёт 15 минут, и claim в нём
/// устарел бы на это время. То есть выдача прав не действовала бы сразу,
/// а снятие прав (или блокировка) не действовало бы вообще до истечения
/// токена. Один запрос вида `select exists(...)` на административную ручку
/// стоит дешевле, чем окно, в котором снятый администратор ещё админ.
///
/// Признак <c>IsAdmin</c> клиент по-прежнему видит в GET /api/users/me —
/// этого достаточно, чтобы решить, показывать ли административный интерфейс.
/// </summary>
public class AdminAuthorizationHandler(
    IUserRepository userRepository,
    ILogger<AdminAuthorizationHandler> logger)
    : AuthorizationHandler<AdminRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        if (!TryGetUserId(context.User, out var userId))
        {
            logger.LogWarning("В токене нет идентификатора пользователя — доступ администратора не выдан");
            return;
        }

        if (await userRepository.IsActiveAdminAsync(userId))
        {
            context.Succeed(requirement);
            return;
        }

        logger.LogInformation(
            "Пользователю {UserId} отказано в доступе к административной ручке",
            userId);
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        // JwtBearer по умолчанию переименовывает claim "sub" в ClaimTypes.NameIdentifier.
        // Читаем оба варианта, иначе проверка молча перестанет находить пользователя,
        // если кто-нибудь выставит MapInboundClaims = false.
        var raw = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return Guid.TryParse(raw, out userId);
    }
}
