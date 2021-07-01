CREATE TABLE [Reminder].[ReminderCertificate]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ReminderId] INT NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [Thumbprint] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_ReminderCertificate_Reminder] FOREIGN KEY ([ReminderId]) REFERENCES [Reminder].[Reminder]([ReminderId])
)

GO

CREATE NONCLUSTERED INDEX [IX_ReminderCertificate_ReminderId] ON [Reminder].[ReminderCertificate] ([ReminderId])
GO
