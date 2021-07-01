
CREATE TABLE [dbo].[FileUploadJobMetaData](
	[Id]				BIGINT			 NOT NULL IDENTITY(1,1),
	[JobId]				BIGINT			 NOT NULL,
	[FileName]			VARCHAR(250)	 NULL,
	[FileSize]			DECIMAL(18, 2)	 NULL,
	[StorageReference]	VARCHAR(100)	 NULL,
    [PeriodNumber]		INT				 NOT NULL DEFAULT 1, 
    --[CreatedOn]        DATETIME       CONSTRAINT [def_dbo_FileUploadJobMetaData_CreatedOn] DEFAULT (getdate()) NULL,
    --[CreatedBy]        NVARCHAR (100) CONSTRAINT [def_dbo_FileUploadJobMetaData_Createdby] DEFAULT (suser_sname()) NULL,
    --[ModifiedOn]       DATETIME       CONSTRAINT [def_dbo_FileUploadJobMetaData_ModifiedOn] DEFAULT (getdate()) NULL,
    --[ModifiedBy]       NVARCHAR (100) CONSTRAINT [def_dbo_FileUploadJobMetaData_ModifiedBy] DEFAULT (suser_sname()) NULL,
    CONSTRAINT [PK_Job_FileUploadJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC ), 
    CONSTRAINT [FK_FileUploadJobMetaData_ToJob] FOREIGN KEY (JobId) REFERENCES [Job](JobId) 
)
GO

CREATE INDEX [IX_FileUploadJobMetaData_Column] ON [dbo].[FileUploadJobMetaData] (JobId)
GO

CREATE NONCLUSTERED INDEX [IX_IlrMetaData_PeriodNumber] ON [dbo].[FileUploadJobMetaData] ([PeriodNumber]) INCLUDE ([JobId],[FileName])
GO
