INSERT INTO [EventStore].[Events] (
    [Version],
    [AggregateRootId],
    [StreamId],
    [TimestampUtc],
    [Type],
    [Data],
    [Metadata])
VALUES (
    @Version,
    @AggregateRootId,
    @StreamId,
    GETDATE(),
    @Type,
    @Data,
    @Metadata)