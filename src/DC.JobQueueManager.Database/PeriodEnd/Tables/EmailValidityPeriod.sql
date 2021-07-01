CREATE TABLE [PeriodEnd].[EmailValidityPeriod]
(
	[HubEmailId]		INT				NOT NULL,
	[HubPathItemId]		INT				NOT NULL DEFAULT 0,
	[CollectionYear]	INT				NOT NULL DEFAULT 0,
	[Period]			INT				NOT NULL,
	[Enabled]			BIT				NOT NULL DEFAULT 1

	CONSTRAINT [PK_EmailValidityPeriod] PRIMARY KEY CLUSTERED ([HubEmailId] ASC, [CollectionYear] ASC, [Period] ASC)
)