CREATE SCHEMA [EventStore]

CREATE TABLE [EventStore].[Events]
(
	[Version] bigint NOT NULL,
	[AggregateRootId] nvarchar(64) NOT NULL,
	[StreamId] nvarchar(64) NOT NULL,
	[TimestampUtc] datetime NOT NULL,
	[Type] nvarchar(512) NOT NULL,
	[Data] nvarchar(max) NOT NULL,
	[Metadata] nvarchar(max) NULL,
    CONSTRAINT [PK_EventStore_Events_Version_AggregateRootId] PRIMARY KEY ([Version], [AggregateRootId])
)