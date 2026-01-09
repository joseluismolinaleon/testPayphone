#pragma warning disable CA1506 // Avoid excessive class coupling
using Asp.Versioning;
using Serilog;
using StackifyLib;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Azure.Identity;
using CustomerValidationSystem.Api.Documentation;
using CustomerValidationSystem.Api.Extensions;
using CustomerValidationSystem.Application;
using CustomerValidationSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ===== Logger con Serilog =====
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ===== Controllers =====
// SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true para compatibilidad con nullable reference types
builder.Services.AddControllers(c => c.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// ===== Azure Key Vault (solo en Production) =====
if (!builder.Environment.IsDevelopment())
{
    var keyVaultName = builder.Configuration["KeyVaultName"];
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// ===== Swagger con Versionado =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});

// ===== Application + Infrastructure =====
// IMPORTANTE: Aquí se registran MediatR, FluentValidation, Repositories y HttpClient
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ===== Health Checks =====
builder.Services.AddHealthChecks();

// ===== Localization (i18n) =====
builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    var defaultCulture = "en-US";
    IList<CultureInfo> supportedCultures =
    [
        new(defaultCulture),
        new("es-ES")
    ];

    opt.DefaultRequestCulture = new RequestCulture(culture: defaultCulture, uiCulture: defaultCulture);
    opt.SupportedCultures = supportedCultures;
    opt.SupportedUICultures = supportedCultures;
    opt.RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()];
});

// ===== Application Insights =====
builder.Services.AddApplicationInsightsTelemetry();

// ===== Build Application =====
var app = builder.Build();

// ===== Configure HTTP Pipeline =====

// Swagger (solo en Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

// ===== Middlewares =====
app.UseRequestLocalization();
app.UseRequestContextLogging();
app.UseSerilogRequestLogging();
app.UseCustomExceptionHandler();

// ===== Stackify =====
app.ConfigureStackifyLogging(app.Configuration);

// ===== Endpoints =====
app.MapControllers();
app.MapHealthChecks("/healthz");

// ===== API Versioning =====
var apiVersion = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var routeGroupBuilder = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersion);

// ===== Run Application =====
await app.RunAsync();

// Necesario para testing con WebApplicationFactory
public partial class Program { }
