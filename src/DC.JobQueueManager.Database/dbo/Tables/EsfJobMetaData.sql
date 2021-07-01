CREATE TABLE [dbo].[EsfJobMetaData]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[JobId] Bigint NOT NULL,
	ContractReferenceNumber nvarchar(20) NOT NULL, 
    [PublishedDate] DATETIME NULL, 
    CONSTRAINT [FK_EsfMetaData_ToTable] FOREIGN KEY ([JobId]) REFERENCES [Job]([JobId])
)
GO
CREATE NONCLUSTERED INDEX [IDX_EsfJobMetaData_JobId] ON [dbo].[EsfJobMetaData] ([JobId])
GO