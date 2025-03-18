using Microsoft.Extensions.DependencyInjection;

namespace PasswordKeeper.BusinessLogic;

/// <summary>
/// The dependency injection extension for the data access.
/// </summary>
public static class DiExtension
{
    /// <summary>
    /// Adds the required services for the data access to the given <paramref name="services"/>.
    /// </summary>
    /// <param name="services">The <see cref="services"/> to add the services to.</param>
    /// <returns>The <paramref name="services"/> with the added services.</returns>
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<Users>();

        return services;
    }
}