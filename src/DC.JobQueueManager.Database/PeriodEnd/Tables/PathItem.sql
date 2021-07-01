CREATE TABLE [PeriodEnd].[PathItem]
(
	[PathItemId]		INT				NOT NULL		IDENTITY(1,1)	PRIMARY KEY,
	[PathId]			INT				NOT NULL,
	[Ordinal]			INT				NOT NULL,
	[IsPausing]			BIT				NOT NULL		DEFAULT(1),
	[HasJobs]			BIT				NOT NULL		DEFAULT(1),
	[Hidden]			BIT				NOT NULL		DEFAULT(0),
	[PathItemLabel]		VARCHAR(MAX)	NOT NULL

	CONSTRAINT [FK_PathItem_Path] FOREIGN KEY ([PathId]) REFERENCES [PeriodEnd].[Path] ([PathId])
)