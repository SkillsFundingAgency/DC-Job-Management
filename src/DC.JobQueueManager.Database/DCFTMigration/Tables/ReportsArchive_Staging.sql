
CREATE TABLE [DCFTMigration].[ReportsArchive_Staging](
	[Id] [int] NOT NULL,
	[UploadedBy] [nvarchar](500) NOT NULL,
	[UploadedDateTimeUtc] [datetime] NOT NULL,
	[Year] [int] NOT NULL,
	[Period] [int] NOT NULL,
	[CollectionTypeId] [int] NOT NULL,
	[inSLD] [bit] NOT NULL,
	[Ukprn] [bigint] NOT NULL,
	[FileName] [nvarchar](max) NULL
) 
GO
