namespace ERP.Purchasing.Application.Common.Models;
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
