namespace ERP.Purchasing.Application.Common.Constants;
public static class CacheKeys
{
    public const string RecentPurchaseOrders = "recent_purchase_orders";
    public const string PurchaseOrderPrefix = "purchase_order_";
    public const int PurchaseOrderCacheCount = 7;

    public static string GetPurchaseOrderKey(Guid id) => $"{PurchaseOrderPrefix}{id}";
    public static string GetPurchaseOrderKey(string number) => $"{PurchaseOrderPrefix}{number}";
}
