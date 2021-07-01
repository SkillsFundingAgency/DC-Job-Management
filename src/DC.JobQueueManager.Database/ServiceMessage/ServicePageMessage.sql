CREATE TABLE [ServiceMessage].[ServicePageMessage] (
	[PageId]				INT					NOT NULL,
	[MessageId]				INT					NOT NULL
	

    CONSTRAINT [PK_ServicePageMessage] PRIMARY KEY CLUSTERED ( [PageId] ASC, [MessageId] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, DATA_COMPRESSION = PAGE)

	CONSTRAINT [FK_ServicePageMessage_ServicePage] FOREIGN KEY ([PageId]) REFERENCES [ServiceMessage].[ServicePage] ([Id])
	CONSTRAINT [FK_ServicePageMessage_ServiceMessage] FOREIGN KEY ([MessageId]) REFERENCES [ServiceMessage].[ServiceMessage] ([Id])
)

GO