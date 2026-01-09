using Asp.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CustomerValidationSystem.Domain.Abstractions;
using CustomerValidationSystem.Infrastructure.Repositories;
using CustomerValidationSystem.Infrastructure.Services;

namespace CustomerValidationSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddVersioning()
            .AddInfrastructureServices(configuration)
            .AddPersistence();

        return services;
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== HttpClient para API Externa =====
        services.AddHttpClient<IReqresApiService, ReqresApiService>(client =>
        {
            // Base URL de la API externa
            var baseUrl = configuration["ReqresApi:BaseUrl"]
                          ?? "https://reqres.in";
            client.BaseAddress = new Uri(baseUrl);

            // API Key en header
            var apiKey = configuration["ReqresApi:ApiKey"]
                         ?? "reqres_5e7b3102c0ef4537bb00f95a52aedc01";
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            // Timeout de 30 segundos
            client.Timeout = TimeSpan.FromSeconds(30);

            // User-Agent (buena práctica)
            client.DefaultRequestHeaders.Add("User-Agent", "CustomerValidationSystem/1.0");
        });

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        // ===== Repositorios con Scoped Lifetime =====
        // Scoped = Una instancia por request HTTP
        // Los datos persisten durante el request pero NO entre requests
        services.AddScoped<IUserRepository, FakeUserRepository>();
        services.AddScoped<ITransactionRepository, FakeTransactionRepository>();

        return services;
    }
}
