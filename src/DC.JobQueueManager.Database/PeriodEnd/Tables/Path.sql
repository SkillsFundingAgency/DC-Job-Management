CREATE TABLE [PeriodEnd].[Path]
(
	[PathId]			INT				NOT NULL	IDENTITY(1,1)	PRIMARY KEY,
	[PeriodEndId]		INT				NOT NULL,
	[HubPathId]			INT				NOT NULL,
	[IsBusy]			BIT				NOT NULL	DEFAULT(0),
	[PathLabel]			VARCHAR(MAX)	NOT NULL,
	[Ordinal]			INT				NOT NULL
	
	CONSTRAINT [FK_Path_PeriodEnd] FOREIGN KEY ([PeriodEndId]) REFERENCES [PeriodEnd].[PeriodEnd] ([PeriodEndId])
)