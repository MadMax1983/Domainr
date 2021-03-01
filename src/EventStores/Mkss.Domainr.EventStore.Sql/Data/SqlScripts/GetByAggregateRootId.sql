SELECT
	[e].[Id],
	[e].[Version],
	[e].[AggregateRootId],
	[e].[Data]
FROM [EventStore].[Events] [e]
WHERE [e].[AggregateRootId] = @AggregateRootId
AND [e].[Version] > @FromVersion