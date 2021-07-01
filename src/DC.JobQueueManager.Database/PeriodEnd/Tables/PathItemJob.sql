CREATE TABLE [PeriodEnd].[PathItemJob]
(
	[JobId]				BIGINT			NOT NULL,
	[PathItemId]		INT				NOT NULL

	CONSTRAINT [PK_PathItemJob] PRIMARY KEY CLUSTERED ([JobId] ASC, [PathItemId] ASC),
	
	CONSTRAINT [FK_PathItemJob_PathItem] FOREIGN KEY ([PathItemId]) REFERENCES [PeriodEnd].[PathItem] ([PathItemId]),
	CONSTRAINT [FK_PathItemJob_Job] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([JobId])
)