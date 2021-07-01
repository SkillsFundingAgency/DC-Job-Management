CREATE TABLE [Reminder].[Reminder]
(
	[ReminderId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(MAX) NULL, 
    [ReminderDate] DATE NULL, 
    [DeadlineDate] DATE NOT NULL, 
    [ClosedDate] DATE NULL, 
    [Notes] NVARCHAR(MAX) NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [UpdatedOn] DATETIME NULL, 
    [CreatedBy] NVARCHAR(250) NOT NULL, 
    [UpdatedBy] NVARCHAR(250) NULL
)
