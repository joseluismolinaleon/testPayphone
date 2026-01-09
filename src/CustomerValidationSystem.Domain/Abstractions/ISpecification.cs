using System.Linq.Expressions;

namespace CustomerValidationSystem.Domain.Abstractions;

public interface ISpecification<TEntity, TEntityId>
where TEntity : Entity<TEntityId>
where TEntityId : IEquatable<TEntityId>
{

    Expression<Func<TEntity, bool>>? Criteria { get; }

    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }

    Expression<Func<TEntity, object>>? OrderBy { get; }

    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    int Take { get; }
    int Skip { get; }

    bool IsPagingEnable { get; }
}
