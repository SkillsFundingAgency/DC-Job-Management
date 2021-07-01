CREATE TABLE [dbo].[EasJobMetaData]
(
	[Id]				BIGINT			 NOT NULL IDENTITY(1,1),
	[JobId]				BIGINT			 NOT NULL,
	[TermsAccepted] BIT NOT NULL, 
    CONSTRAINT [PK_Job_EasJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC ), 
    CONSTRAINT [FK_EasJobMetaData_ToJob] FOREIGN KEY (JobId) REFERENCES [Job](JobId) 
)
GO
CREATE NONCLUSTERED INDEX [IDX_EasJobMetaData_JobId] ON [dbo].[EasJobMetaData] ([JobId])
GO
