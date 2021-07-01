CREATE TABLE [PeriodEnd].[PeriodEnd]
(
	[PeriodEndId]						INT			NOT NULL		IDENTITY(1,1)	PRIMARY KEY,
	[PeriodId]							INT			NOT NULL,
	[ProviderReportsReady]				BIT			NOT NULL		DEFAULT(0),
	[ProviderReportsPublished]			BIT			NOT NULL		DEFAULT(0),
	[FM36ReportsReady]					BIT			NOT NULL		DEFAULT(0),
	[FM36ReportsPublished]				BIT			NOT NULL		DEFAULT(0),
	[MCAReportsReady]					BIT			NOT NULL		DEFAULT(0),
	[MCAReportsPublished]				BIT			NOT NULL		DEFAULT(0),
	[Closed]							BIT			NOT NULL		DEFAULT(0),
	[PeriodEndStarted]					DATETIME,
	[PeriodEndFinished]					DATETIME,
	[CollectionClosedEmailSent]			BIT			NOT NULL		DEFAULT (0),
	[FrmReportsPublished] BIT NOT NULL DEFAULT (0)

	CONSTRAINT [FK_PeriodEnd_Period] FOREIGN KEY ([PeriodId]) REFERENCES [dbo].[ReturnPeriod] ([ReturnPeriodId]), 
    [EsfSummarisationFinished] DATETIME NULL, 
    [DcSummarisationFinished] DATETIME NULL, 
    [AppsSummarisationFinished] DATETIME NULL, 
    
)