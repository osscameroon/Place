using Account.Data.Configurations;
using Account.Data.Seed;
using Core.EF;
using Core.Framework;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account;

public static class AccountModule
{
    public static IServiceCollection AddAccountModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPlaceDbContext<AccountDbContext>(nameof(Account), configuration);
        services.AddPlaceDbContext<IdentityApplicationDbContext>(
            nameof(Core.Identity),
            configuration
        );
        services.AddScoped<IDataSeeder, AccountDataSeeder>();
        return services;
    }

    public static WebApplication UseAccountModule(
        this WebApplication app,
        IWebHostEnvironment environment
    )
    {
        app.UseCoreFramework();
        app.UseMigration<AccountDbContext>(environment);
        return app;
    }
}
