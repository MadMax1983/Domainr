INSERT INTO [EventStore].[Events] ([Id], [Version], [AggregateRootId], [Type], [Data])
VALUES (@Id, @Version, @AggregateRootId, @Type, @Data)