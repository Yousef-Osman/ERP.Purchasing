CREATE TABLE [dbo].[PurchaseOrders]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Number] NVARCHAR(50) NOT NULL UNIQUE,
    [IssueDate] DATETIME2 NOT NULL,
    [TotalPrice] DECIMAL(18, 2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [State] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [RowVersion] ROWVERSION NOT NULL
);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_Number] ON [dbo].[PurchaseOrders]([Number]);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_State] ON [dbo].[PurchaseOrders]([State]);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_IsActive] ON [dbo].[PurchaseOrders]([IsActive]);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_IssueDate] ON [dbo].[PurchaseOrders]([IssueDate] DESC);

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_State_IsActive] ON [dbo].[PurchaseOrders]([State], [IsActive]) INCLUDE ([IssueDate], [TotalPrice]);