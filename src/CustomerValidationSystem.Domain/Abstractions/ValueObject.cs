using CustomerValidationSystem.SharedKernel.Guards;

namespace CustomerValidationSystem.Domain.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);

    public virtual bool Equals(ValueObject? other) => other is not null && this.ValuesAreEqual(other);

    public override bool Equals(object? obj) => obj is ValueObject valueObject && this.ValuesAreEqual(valueObject);

    public override int GetHashCode() =>
        this.GetAtomicValues().Aggregate(default(int), (hashcode, value) => HashCode.Combine(hashcode, value.GetHashCode()));

    protected abstract IEnumerable<object> GetAtomicValues();

    protected bool ValuesAreEqual(ValueObject valueObject)
    {
        Guard.ThrowIfNull(valueObject);
        return this.GetAtomicValues().SequenceEqual(valueObject.GetAtomicValues());
    }
}
