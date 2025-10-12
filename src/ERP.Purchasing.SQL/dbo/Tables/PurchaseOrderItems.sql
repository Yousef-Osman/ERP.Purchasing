CREATE TABLE [dbo].[PurchaseOrderItems]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [SerialNumber] INT NOT NULL,
    [GoodCode] NVARCHAR(50) NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [PurchaseOrderId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders] 
        FOREIGN KEY ([PurchaseOrderId]) 
        REFERENCES [dbo].[PurchaseOrders]([Id]) 
        ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderItems_PurchaseOrderId] ON [dbo].[PurchaseOrderItems]([PurchaseOrderId]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PurchaseOrderItems_PurchaseOrderId_GoodCode] ON [dbo].[PurchaseOrderItems]([PurchaseOrderId], [GoodCode]);
