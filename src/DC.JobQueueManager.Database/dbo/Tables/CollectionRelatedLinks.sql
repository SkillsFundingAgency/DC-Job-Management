CREATE TABLE [dbo].[CollectionRelatedLink](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CollectionId] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[SortOrder] [int] NOT NULL,
  CONSTRAINT [PK_CollectionRelatedLink] PRIMARY KEY CLUSTERED ([Id] ASC ), 
  CONSTRAINT [FK_CollectionRelatedLink_Collection] FOREIGN KEY (CollectionId) REFERENCES [Collection](CollectionId) 
  )
