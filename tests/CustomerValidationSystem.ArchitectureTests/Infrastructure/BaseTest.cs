using System.Reflection;

namespace CustomerValidationSystem.ArchitectureTests.Infrastructure;

public class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(CustomerValidationSystem.Domain.Abstractions.Result).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(CustomerValidationSystem.Infrastructure.DependencyInjection).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
