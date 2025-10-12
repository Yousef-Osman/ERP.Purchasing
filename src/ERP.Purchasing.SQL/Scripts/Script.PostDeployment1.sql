/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--seed PurchaseOrders and PurchaseOrderItems
--------------------------------------------------------------------------------------
DECLARE @Counter INT = 1;
DECLARE @POId UNIQUEIDENTIFIER;
DECLARE @ItemId UNIQUEIDENTIFIER;

WHILE @Counter <= 100
BEGIN
    SET @POId = NEWID();
    
    INSERT INTO [dbo].[PurchaseOrders] ([Id], [Number], [IssueDate], [TotalPrice], [Currency], [State], [IsActive])
    VALUES (
        @POId,
        'PO' + RIGHT('000000' + CAST(@Counter AS VARCHAR(6)), 6),
        DATEADD(DAY, -(@Counter % 365), GETUTCDATE()),
        (@Counter * 100.50),
        'USD',
        (@Counter % 4) + 1, -- Cycle through states
        CASE WHEN @Counter % 10 = 0 THEN 0 ELSE 1 END -- 10% inactive
    );
    
    -- Add 2-5 items per PO
    DECLARE @ItemCount INT = 2 + (@Counter % 4);
    DECLARE @ItemCounter INT = 1;
    
    WHILE @ItemCounter <= @ItemCount
    BEGIN
        SET @ItemId = NEWID();
        
        INSERT INTO [dbo].[PurchaseOrderItems] ([Id], [SerialNumber], [GoodCode], [Price], [Currency], [PurchaseOrderId])
        VALUES (
            @ItemId,
            @ItemCounter,
            'GOOD' + RIGHT('000' + CAST(@ItemCounter AS VARCHAR(3)), 3),
            (@ItemCounter * 50.25),
            'USD',
            @POId
        );
        
        SET @ItemCounter = @ItemCounter + 1;
    END
    
    SET @Counter = @Counter + 1;
END

GO
