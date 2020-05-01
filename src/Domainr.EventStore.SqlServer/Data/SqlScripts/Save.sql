INSERT INTO [EventStore].[Events] ([Id], [Version], [AggregateRootId], [Data])
VALUES (@Id, @Version, @AggregateRootId, @Data)