namespace ERP.Purchasing.API.Moddels;

public class UpdatePriceRequest
{
    public decimal NewPrice { get; set; }
    public string Currency { get; set; } = "USD";
}