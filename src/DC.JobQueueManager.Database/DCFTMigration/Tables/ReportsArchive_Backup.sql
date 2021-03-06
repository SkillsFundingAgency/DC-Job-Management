
CREATE TABLE [DCFTMigration].[ReportsArchive_Backup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UploadedBy] [nvarchar](500) NOT NULL,
	[UploadedDateTimeUtc] [datetime] NOT NULL,
	[Year] [int] NOT NULL,
	[Period] [int] NOT NULL,
	[CollectionTypeId] [int] NOT NULL,
	[inSLD] [bit] NOT NULL,
	[Ukprn] [bigint] NOT NULL DEFAULT (0),
	[FileName] [nvarchar](max) NULL DEFAULT (''),
  CONSTRAINT [PK_DCFTMigration_ReportsArchive] PRIMARY KEY CLUSTERED
  ( [Id] ASC ) 
  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO