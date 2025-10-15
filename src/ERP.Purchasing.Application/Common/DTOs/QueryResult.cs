namespace ERP.Purchasing.Application.Common.DTOs;
public class QueryResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }

    public QueryResult(IEnumerable<T> items, int totalCount)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
    }
}
