using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlaceAPi.Identity.Authenticate;

namespace PlaceApi.Identity.Tests.Integration.Common;

/// <summary>
/// Helper class for managing users in integration tests.
/// Provides methods for creating and manipulating user accounts.
/// </summary>
public class UserManagementHelper
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the UserManagementHelper class.
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider to resolve dependencies.</param>
    public UserManagementHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new user with the specified email and password.
    /// </summary>
    /// <param name="email">The email address for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="emailConfirmed">Whether the email should be marked as confirmed. Default is true.</param>
    /// <returns>The created ApplicationUser.</returns>
    public async Task<ApplicationUser> CreateUserAsync(
        string email,
        string password,
        bool emailConfirmed = true
    )
    {
        UserManager<ApplicationUser> userManager = _serviceProvider.GetRequiredService<
            UserManager<ApplicationUser>
        >();

        ApplicationUser? existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            await userManager.DeleteAsync(existingUser);
        }

        ApplicationUser user =
            new()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = emailConfirmed,
            };
        IdentityResult createResult = await userManager.CreateAsync(user, password);
        createResult.Succeeded.Should().BeTrue("user creation should succeed");
        return user;
    }

    /// <summary>
    /// Locks out the specified user.
    /// </summary>
    /// <param name="user">The user to lock out.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LockOutUserAsync(ApplicationUser user)
    {
        UserManager<ApplicationUser> userManager = _serviceProvider.GetRequiredService<
            UserManager<ApplicationUser>
        >();
        await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddHours(1));
    }

    public async Task VerifyUserCreatedAsync(string email)
    {
        UserManager<ApplicationUser> userManager = _serviceProvider.GetRequiredService<
            UserManager<ApplicationUser>
        >();
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
    }
}
