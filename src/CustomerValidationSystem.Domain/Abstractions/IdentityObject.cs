namespace CustomerValidationSystem.Domain.Abstractions;

public abstract class IdentityObject<TIdentity> : ValueObject, IEquatable<TIdentity> where TIdentity : ValueObject
{
    public virtual bool Equals(TIdentity? other) => other is not null && this.ValuesAreEqual(other);
}
