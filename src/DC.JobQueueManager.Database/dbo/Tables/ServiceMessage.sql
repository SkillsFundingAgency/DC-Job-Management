
CREATE TABLE [dbo].[ServiceMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[StartDateTimeUtc] [datetime] NOT NULL,
	[EndDateTimeUtc] [datetime] NULL,
	[Headline] VARCHAR(50) NULL, 
    CONSTRAINT [PK_ServiceMessage] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, DATA_COMPRESSION = PAGE)
)
GO
