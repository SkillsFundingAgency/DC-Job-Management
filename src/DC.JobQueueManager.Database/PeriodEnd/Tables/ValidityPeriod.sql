CREATE TABLE [PeriodEnd].[ValidityPeriod]
(
	[HubPathItemId]		INT				NOT NULL DEFAULT 0,
	[Period]			INT				NOT NULL,
	[CollectionYear]	INT				NOT NULL DEFAULT 0,
	[Enabled]			BIT				NOT NULL DEFAULT 1

	CONSTRAINT [PK_ValidityPeriod] PRIMARY KEY CLUSTERED ([HubPathItemId] ASC, [CollectionYear] ASC, [Period] ASC)
)