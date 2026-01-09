namespace CustomerValidationSystem.Domain.Abstractions;

public class PagedResults<TEntity, TEntityId>
where TEntity : Entity<TEntityId>
where TEntityId : IEquatable<TEntityId>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalNumberOfPages { get; set; }
    public int TotalNumberOfRecords { get; set; }
    public IReadOnlyList<TEntity> Results { get; set; }
    public PagedResults(int pageNumber, int pageSize, int totalNumberOfPages, int totalNumberOfRecords, IReadOnlyList<TEntity> results)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.TotalNumberOfPages = totalNumberOfPages;
        this.TotalNumberOfRecords = totalNumberOfRecords;
        this.Results = results;
    }
}
