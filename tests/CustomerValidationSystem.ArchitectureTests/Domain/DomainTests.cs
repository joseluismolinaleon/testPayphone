using CustomerValidationSystem.ArchitectureTests.Infrastructure;
using CustomerValidationSystem.Domain.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace CustomerValidationSystem.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{

    [Fact]
    public void Entities_ShoulHave_PrivateConstructorNoParameteres()
    {
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity<>))
            .GetTypes();

        var errorEntities = new List<Type>();

        foreach (var entityType in entityTypes)
        {
            var constructores = entityType.GetConstructors(
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            if (!constructores.Any(c => (c.IsPrivate || c.IsFamily) && c.GetParameters().Length == 0))
            {
                errorEntities.Add(entityType);
            }
        }

        errorEntities.Should().BeEmpty();

    }

}
