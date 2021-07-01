CREATE TABLE [PeriodEnd].[SubPathValidityPeriod]
(
	[HubPathId]			INT				NOT NULL,
	[Period]			INT				NOT NULL,
	[CollectionYear]	INT				NOT NULL DEFAULT 0,
	[Enabled]			BIT				NOT NULL DEFAULT 1

	CONSTRAINT [PK_SubPathValidityPeriod] PRIMARY KEY CLUSTERED ([HubPathId] ASC, [CollectionYear] ASC, [Period] ASC)
)