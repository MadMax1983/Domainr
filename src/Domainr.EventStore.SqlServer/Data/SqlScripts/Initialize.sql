CREATE SCHEMA [EventStpre]

CREATE TABLE [EventStore].[Events]
(
	[Id] nvarchar(64) NOT NULL,
	[Version] bigint NOT NULL,
	[AggregateRootId] nvarchar(64) NOT NULL,
	[Data] nvarchar(max) NOT NULL,
)