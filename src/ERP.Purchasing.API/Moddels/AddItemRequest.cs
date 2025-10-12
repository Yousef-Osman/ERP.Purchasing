namespace ERP.Purchasing.API.Moddels;

public class AddItemRequest
{
    public string GoodCode { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
}