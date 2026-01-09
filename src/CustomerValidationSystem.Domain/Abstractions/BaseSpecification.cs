using System.Linq.Expressions;

namespace CustomerValidationSystem.Domain.Abstractions;

public abstract class BaseSpecification<TEntity, TEntityId>
    : ISpecification<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<TEntity, bool>>? criteria) => this.Criteria = criteria;

    public Expression<Func<TEntity, bool>>? Criteria { get; }

    private readonly List<Expression<Func<TEntity, object>>> _includes = new();
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => this._includes;

    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool IsPagingEnable { get; private set; }

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression) => this.OrderBy = orderByExpression;

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression) => this.OrderByDescending = orderByDescendingExpression;

    protected void ApplyPaging(int skip, int take)
    {
        this.Skip = skip;
        this.Take = take;
        this.IsPagingEnable = true;
    }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) => this._includes.Add(includeExpression);
}
