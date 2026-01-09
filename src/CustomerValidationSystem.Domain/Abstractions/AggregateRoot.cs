namespace CustomerValidationSystem.Domain.Abstractions;

public abstract class AggregateRoot<TEntityId> : Entity<TEntityId> where TEntityId : IEquatable<TEntityId>
{
    protected AggregateRoot(TEntityId id) : base(id)
    {
    }

    protected AggregateRoot() { }

    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => this._domainEvents.ToList();

    public void ClearDomainEvents() => this._domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => this._domainEvents.Add(domainEvent);
}
