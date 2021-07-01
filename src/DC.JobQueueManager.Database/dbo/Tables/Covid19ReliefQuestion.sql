CREATE TABLE [dbo].[Covid19ReliefQuestion]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [QuestionNumber] varchar(100) NOT NULL, 
    [Answer] NVARCHAR(MAX) NOT NULL, 
    [Covid19ReliefSubmissionId] INT NOT NULL,
	    [AnswerType] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Covid19ReliefSubmissionData_ToCovid19ReliefSubmissionData] FOREIGN KEY (Covid19ReliefSubmissionId) REFERENCES dbo.Covid19ReliefSubmission([Covid19ReliefSubmissionId]),

)
