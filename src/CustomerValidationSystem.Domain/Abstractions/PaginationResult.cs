namespace CustomerValidationSystem.Domain.Abstractions;

public class PaginationResult<TEntity, TEntityId>
where TEntity : Entity<TEntityId>
where TEntityId : IEquatable<TEntityId>
{

    public int Count { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public IReadOnlyList<TEntity>? Data { get; set; }

    public int PageCount { get; set; }

    public int ResultByPage { get; set; }
}
