SELECT *
FROM [EventStore].[Events] [e]
WHERE [e].[AggregateRootId] = @AggregateRootId
AND [e].[Version] >= @FromVersion