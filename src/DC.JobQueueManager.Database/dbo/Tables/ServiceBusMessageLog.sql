CREATE TABLE [dbo].[ServiceBusMessageLog]
(
	[Id] INT NOT NULL IDENTITY, 
    [JobId] BIGINT NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [DateTimeCreatedUtc] DATETIME NOT NULL,
    CONSTRAINT [PK_ServiceBusMessageLog] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE),
)
GO
CREATE INDEX [IDX_ServiceBusMessageLog_JobId] ON [dbo].[ServiceBusMessageLog] ([JobId])
GO