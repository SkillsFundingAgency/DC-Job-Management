
CREATE TABLE [dbo].[ValidationRuleDetailsReportJobMetaData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Rule] [nvarchar](100) NOT NULL,
	[SelectedCollectionYear] int NOT NULL,
	[JobId] [bigint] NOT NULL,
	CONSTRAINT [PK_Job_ValidationRuleDetailsReportJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC ), 
    CONSTRAINT [FK_ValidationRuleDetailsReportJobMetaData_ToJob] FOREIGN KEY (JobId) REFERENCES [Job](JobId) 
)

