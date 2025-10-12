CREATE TABLE [dbo].[ProcessedEvents]
(
    [EventId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ProcessedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [EventType] NVARCHAR(200) NOT NULL
);

GO
CREATE NONCLUSTERED INDEX [IX_ProcessedEvents_ProcessedAt] ON [dbo].[ProcessedEvents]([ProcessedAt] DESC);
