CREATE TABLE [dbo].[Schedule] (
    [ID]					BIGINT   IDENTITY (1, 1) NOT NULL,
    [Enabled]				BIT      NOT NULL,
    [MinuteIsCadence]		BIT      NULL,
    [Minute]				TINYINT  NULL,
    [Hour]					TINYINT  NULL,
    [DayOfTheMonth]			TINYINT  NULL,
    [Month]					TINYINT  NULL,
    [DayOfTheWeek]			TINYINT	 NULL,
    [CollectionId]          INT		 NOT NULL DEFAULT 1,
    [ExecuteOnceOnly]		BIT      NOT NULL,
    [LastExecuteDateTime]	DATETIME NULL,
	[Paused]				BIT		 NOT NULL DEFAULT 0

    CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Schedule_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [dbo].[Collection] ([CollectionId])
);