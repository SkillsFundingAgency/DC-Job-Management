CREATE TABLE [dbo].[Covid19ReliefReviewComment]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Comment] NVARCHAR(MAX) NOT NULL, 
    [AddedBy] NVARCHAR(MAX) NOT NULL, 
    [DateTimeAddedUtc] DATETIME NOT NULL, 
    [Covid19ReliefSubmissionId] INT NOT NULL, 
    [IsApproved] BIT NULL, 
    [ApprovedDateTimeUtc] DATETIME NULL
)
