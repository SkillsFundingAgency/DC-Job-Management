CREATE TABLE [Mailing].[Recipient]
(
	[RecipientId]				INT				NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[EmailAddress]				VARCHAR(500),
	CONSTRAINT UK_EmailAddress UNIQUE(EmailAddress)
)