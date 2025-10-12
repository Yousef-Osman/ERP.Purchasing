namespace ERP.Purchasing.Infrastructure.Persistence.Entities;
public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string EventType { get; set; }
}
