CREATE TABLE [dbo].[NcsJobMetaData]
(
	[Id]				BIGINT			 NOT NULL IDENTITY(1,1),
	[JobId]				BIGINT			 NOT NULL,
	[ExternalJobId]		VARCHAR(250)	 NOT NULL,
	[TouchpointId]		VARCHAR(250)	 NOT NULL,
	[ExternalTimestamp]	DATETIME		 NOT NULL,
	[ReportFileName]	VARCHAR(250)	 NOT NULL,
	[DssContainer]		VARCHAR(250)		NOT NULL, 
    [ReportEndDate]		DATETIME			NOT NULL, 
    [PeriodNumber] INT NOT NULL, 
    CONSTRAINT [PK_Job_NcsJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC ), 
    CONSTRAINT [FK_NcsJobMetaData_ToJob] FOREIGN KEY (JobId) REFERENCES [Job](JobId) 
)
GO
CREATE NONCLUSTERED INDEX [IDX_NcsJobMetaData_JobId] ON [dbo].[NcsJobMetaData] ([JobId])
GO