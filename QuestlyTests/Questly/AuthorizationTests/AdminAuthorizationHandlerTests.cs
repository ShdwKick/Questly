using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Questly.Authorization;
using Questly.Repositories;

namespace QuestlyTests.Questly.AuthorizationTests;

public class AdminAuthorizationHandlerTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static AuthorizationHandlerContext CreateContext(ClaimsPrincipal principal)
    {
        var requirement = new AdminRequirement();
        return new AuthorizationHandlerContext([requirement], principal, resource: null);
    }

    private static ClaimsPrincipal Authenticated(params Claim[] claims)
        => new(new ClaimsIdentity(claims, "TestAuth"));

    private static AdminAuthorizationHandler CreateHandler(
        Guid? expectedUserId,
        bool isActiveAdmin,
        out Mock<IUserRepository> repository)
    {
        repository = new Mock<IUserRepository>(MockBehavior.Strict);

        if (expectedUserId.HasValue)
        {
            repository
                .Setup(r => r.IsActiveAdminAsync(expectedUserId.Value))
                .ReturnsAsync(isActiveAdmin);
        }

        return new AdminAuthorizationHandler(
            repository.Object,
            NullLogger<AdminAuthorizationHandler>.Instance);
    }

    [Fact]
    public async Task Succeeds_WhenUserIsActiveAdmin()
    {
        var handler = CreateHandler(UserId, isActiveAdmin: true, out _);
        var context = CreateContext(Authenticated(new Claim(ClaimTypes.NameIdentifier, UserId.ToString())));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Fails_WhenUserIsNotAdmin()
    {
        var handler = CreateHandler(UserId, isActiveAdmin: false, out _);
        var context = CreateContext(Authenticated(new Claim(ClaimTypes.NameIdentifier, UserId.ToString())));

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    /// <summary>
    /// JwtBearer переименовывает "sub" в ClaimTypes.NameIdentifier, но если
    /// отключить MapInboundClaims, в токене останется исходное имя claim.
    /// Обработчик должен понимать оба варианта.
    /// </summary>
    [Fact]
    public async Task Succeeds_WhenIdentifierComesAsRawSubClaim()
    {
        var handler = CreateHandler(UserId, isActiveAdmin: true, out _);
        var context = CreateContext(Authenticated(new Claim(JwtRegisteredClaimNames.Sub, UserId.ToString())));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Fails_AndDoesNotHitDatabase_WhenTokenHasNoUserId()
    {
        var handler = CreateHandler(expectedUserId: null, isActiveAdmin: false, out var repository);
        var context = CreateContext(Authenticated(new Claim("unrelated", "value")));

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        repository.Verify(r => r.IsActiveAdminAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Fails_AndDoesNotHitDatabase_WhenUserIdIsNotGuid()
    {
        var handler = CreateHandler(expectedUserId: null, isActiveAdmin: false, out var repository);
        var context = CreateContext(Authenticated(new Claim(ClaimTypes.NameIdentifier, "not-a-guid")));

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        repository.Verify(r => r.IsActiveAdminAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Fails_WhenRequestIsAnonymous()
    {
        var handler = CreateHandler(expectedUserId: null, isActiveAdmin: false, out var repository);
        var context = CreateContext(new ClaimsPrincipal(new ClaimsIdentity()));

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        repository.Verify(r => r.IsActiveAdminAsync(It.IsAny<Guid>()), Times.Never);
    }
}
