using DataModels;
using DataModels.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Questly.Repositories;
using Questly.Services;

namespace QuestlyTests.Questly.ServicesTests;

public class UserServiceAdminTests
{
    private static readonly Guid CurrentUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid OtherUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static UserService CreateService(
        out Mock<IUserRepository> userRepository,
        Guid? currentUserId = null)
    {
        userRepository = new Mock<IUserRepository>();

        var headerHelper = new Mock<IHeaderHelper>();
        headerHelper
            .Setup(h => h.GetUserIdFromHeader())
            .Returns(currentUserId ?? CurrentUserId);

        return new UserService(
            userRepository.Object,
            Mock.Of<IAuthorizationRepository>(),
            NullLogger<UserService>.Instance,
            headerHelper.Object);
    }

    [Fact]
    public async Task SetAdminAsync_GrantsRights_ToAnotherUser()
    {
        var service = CreateService(out var userRepository);
        userRepository
            .Setup(r => r.SetAdminAsync(OtherUserId, true))
            .ReturnsAsync(new User { Id = OtherUserId, IsAdmin = true });

        var user = await service.SetAdminAsync(OtherUserId, true);

        Assert.True(user.IsAdmin);
        userRepository.Verify(r => r.SetAdminAsync(OtherUserId, true), Times.Once);
    }

    [Fact]
    public async Task SetAdminAsync_RevokesRights_FromAnotherUser()
    {
        var service = CreateService(out var userRepository);
        userRepository
            .Setup(r => r.SetAdminAsync(OtherUserId, false))
            .ReturnsAsync(new User { Id = OtherUserId, IsAdmin = false });

        var user = await service.SetAdminAsync(OtherUserId, false);

        Assert.False(user.IsAdmin);
        userRepository.Verify(r => r.SetAdminAsync(OtherUserId, false), Times.Once);
    }

    /// <summary>
    /// Снятие прав с самого себя заблокировано: единственный администратор
    /// иначе закрывает себе доступ к управлению контентом одним запросом.
    /// </summary>
    [Fact]
    public async Task SetAdminAsync_Throws_WhenRevokingOwnRights()
    {
        var service = CreateService(out var userRepository);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.SetAdminAsync(CurrentUserId, false));

        Assert.Contains("CANNOT_REVOKE_OWN_ADMIN_PROBLEM", exception.Message);
        userRepository.Verify(r => r.SetAdminAsync(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
    }

    /// <summary>
    /// Выдать права самому себе — не проблема: администратор ими уже обладает,
    /// иначе он бы не дошёл до этой ручки.
    /// </summary>
    [Fact]
    public async Task SetAdminAsync_Allows_GrantingToSelf()
    {
        var service = CreateService(out var userRepository);
        userRepository
            .Setup(r => r.SetAdminAsync(CurrentUserId, true))
            .ReturnsAsync(new User { Id = CurrentUserId, IsAdmin = true });

        var user = await service.SetAdminAsync(CurrentUserId, true);

        Assert.True(user.IsAdmin);
    }

    [Fact]
    public async Task SetAdminAsync_Throws_WhenUserIdIsEmpty()
    {
        var service = CreateService(out var userRepository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SetAdminAsync(Guid.Empty, true));

        userRepository.Verify(r => r.SetAdminAsync(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
    }
}
