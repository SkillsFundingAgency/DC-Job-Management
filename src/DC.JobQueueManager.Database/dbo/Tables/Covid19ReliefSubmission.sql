CREATE TABLE [dbo].[Covid19ReliefSubmission]
(
	[Covid19ReliefSubmissionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FileName] NVARCHAR(MAX) NOT NULL, 
    [DateTimeSubmittedUtc] DATETIME NOT NULL, 
    [SubmittedBy] NVARCHAR(MAX) NOT NULL, 
    [CollectionId] INT NOT NULL, 
    [ReturnPeriodId] INT NOT NULL, 
    [Ukprn] BIGINT NOT NULL, 
    [ProviderName] NVARCHAR(MAX) NOT NULL, 
    [Address] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_Covid19ReliefMetaData_ToCollection] FOREIGN KEY ([CollectionId]) REFERENCES dbo.[Collection]([CollectionId]),
	CONSTRAINT [FK_Covid19ReliefMetaData_ToReturnPeriod] FOREIGN KEY ([ReturnPeriodId]) REFERENCES dbo.[ReturnPeriod]([ReturnPeriodId])
)
