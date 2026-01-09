using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerValidationSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ===== MediatR =====
        // Registra todos los handlers (Commands y Queries) automáticamente
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // ===== FluentValidation =====
        // Registra todos los validators automáticamente
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
