namespace ERP.Purchasing.Application.Common.DTOs;
public class CreatePurchaseOrderItemDto
{
    public string GoodCode { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
}
