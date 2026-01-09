namespace CustomerValidationSystem.Domain.Abstractions;

public abstract class Entity<TEntityId> : IEntity<TEntityId> where TEntityId : IEquatable<TEntityId>
{
    protected Entity()
    {
    }

    protected Entity(TEntityId id) => this.Id = id;

    public TEntityId Id { get; init; } = default!;

}
