CREATE TABLE [Mailing].[EmailRecipientGroup]
(
	[EmailRecipientGroupId]		INT			NOT NULL IDENTITY(1,1)	 PRIMARY KEY,
	[EmailId]					INT			NOT NULL,
	[RecipientGroupId]			INT

	CONSTRAINT [FK_EmailRecipientGroup_Email] FOREIGN KEY ([EmailId]) REFERENCES [Mailing].[Email] ([EmailId]),
	CONSTRAINT [FK_EmailRecipientGroup_RecipientGroup] FOREIGN KEY ([RecipientGroupId]) REFERENCES [Mailing].[RecipientGroup] ([RecipientGroupId])
)
