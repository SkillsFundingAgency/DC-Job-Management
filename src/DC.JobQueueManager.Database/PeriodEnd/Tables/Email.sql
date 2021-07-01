CREATE TABLE [Mailing].[Email]
(
	[EmailId]			INT					NOT NULL		IDENTITY(1,1)		PRIMARY KEY,
	[HubEmailId]		INT					NOT NULL,
	[TemplateId]		VARCHAR(250)		NOT NULL,
	[TemplateName]				VARCHAR(250),
	[TriggerPointName]				VARCHAR(250)
)