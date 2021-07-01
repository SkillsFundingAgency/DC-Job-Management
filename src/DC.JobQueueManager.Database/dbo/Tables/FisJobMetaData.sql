CREATE TABLE [dbo].[FisJobMetaData]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[JobId] Bigint NOT NULL,
	[VersionNumber] INT NOT NULL, 
    [GeneratedDate] DATETIME NULL, 
    [PublishedDate] DATETIME NULL, 
    [IsRemoved] BIT NULL, 
    CONSTRAINT [FK_FisJobMetaData_Job] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId])
)
GO
CREATE NONCLUSTERED INDEX [IDX_FisJobMetaData_JobId] ON [dbo].[FisJobMetaData] ([JobId])
GO