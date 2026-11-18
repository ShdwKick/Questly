using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questly.Authorization;
using Questly.Extensions;

namespace QuestlyTests.Questly.AuthorizationTests;

/// <summary>
/// Проверяет, что политика Admin и её обработчик реально собираются из DI.
/// Ошибка регистрации иначе проявилась бы только на живом запросе к серверу:
/// unit-тесты обработчика конструируют его руками и такую ошибку не ловят.
/// Подключение к базе здесь не открывается — DbContext регистрируется лениво.
/// </summary>
public class AuthorizationWiringTests
{
    private static ServiceProvider BuildProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Host=localhost;Database=questly;Username=u;Password=p",
                ["SERVER_KEY"] = "test-server-key-which-is-long-enough-32",
                ["ISSUER"] = "https://test.local",
                ["AUDIENCE"] = "https://test.local"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        // В реальном приложении IConfiguration регистрирует хост,
        // в чистом ServiceCollection её нужно положить руками.
        services.AddSingleton<IConfiguration>(configuration);
        services.AddQuestlyConfiguration(configuration);
        services.AddQuestlyServices();
        services.AddQuestlyAuthentication(configuration);
        services.AddQuestlyAuthorization();

        return services.BuildServiceProvider(validateScopes: true);
    }

    [Fact]
    public async Task AdminPolicy_IsRegistered_WithAdminRequirement()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var policyProvider = scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await policyProvider.GetPolicyAsync(Policies.Admin);

        Assert.NotNull(policy);
        Assert.Contains(policy.Requirements, r => r is AdminRequirement);
    }

    [Fact]
    public void AdminHandler_CanBeResolved()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IAuthorizationHandler>();

        Assert.Contains(handlers, h => h is AdminAuthorizationHandler);
    }
}
