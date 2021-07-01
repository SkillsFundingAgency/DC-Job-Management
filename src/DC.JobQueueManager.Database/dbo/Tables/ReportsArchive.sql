CREATE TABLE [dbo].[ReportsArchive](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UploadedBy] [nvarchar](500) NOT NULL,
	[UploadedDateTimeUtc] [datetime] NOT NULL,
	[Year] [int] NOT NULL,
	[Period] [int] NOT NULL,
	[CollectionTypeId] [int] NOT NULL,
	[inSLD] [bit] NOT NULL,
	[Ukprn] BIGINT NOT NULL DEFAULT 0, 
    [FileName] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_ReportsArchive] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReportsArchive]  WITH CHECK ADD  CONSTRAINT [FK_ReportsArchive_CollectionType] FOREIGN KEY([CollectionTypeId])
REFERENCES [dbo].[CollectionType] ([CollectionTypeId])
GO

ALTER TABLE [dbo].[ReportsArchive] CHECK CONSTRAINT [FK_ReportsArchive_CollectionType]
GO


