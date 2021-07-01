CREATE TABLE [Mailing].[RecipientGroupRecipient]
(
	[RecipientGroupId]						INT		NOT NULL,
	[RecipientId]							INT		NOT NULL

	CONSTRAINT [PK_RecipientGroupRecipient] PRIMARY KEY CLUSTERED ([RecipientGroupId] ASC, [RecipientId]  ASC),

    CONSTRAINT [FK_RecipientGroupRecipient_RecipientGroup] FOREIGN KEY ([RecipientGroupId]) REFERENCES [Mailing].[RecipientGroup] ([RecipientGroupId]),
	CONSTRAINT [FK_RecipientGroupRecipient_Recipient] FOREIGN KEY ([RecipientId]) REFERENCES [Mailing].[Recipient] ([RecipientId])
)
