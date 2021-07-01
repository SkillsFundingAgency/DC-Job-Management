
CREATE TABLE [dbo].[ReportsPublicationJobMetaData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceFolderKey] [nvarchar](100) NOT NULL,
	[SourceContainerName] [nvarchar](100) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[PeriodNumber] INT NOT NULL DEFAULT 1, 
    [StorageReference] VARCHAR(100) NULL, 
	[ReportsPublished] bit NULL
    CONSTRAINT [PK_Job_ReportsPublicationJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC ), 
    CONSTRAINT [FK_ReportsPublicationJobMetaData_ToJob] FOREIGN KEY (JobId) REFERENCES [Job](JobId) 
)

