CREATE TABLE [dbo].[ReturnPeriodDisplayOverride] (
    [Id]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [ReturnPeriodId]   INT      NOT NULL,
    [StartDateTimeUtc] DATETIME NULL,
    [EndDateTimeUtc]   DATETIME NULL,
    [PeriodNumber]     INT      NULL,
    CONSTRAINT [PK_ReturnPeriodOverride] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReturnPeriodOverride_ReturnPeriod] FOREIGN KEY ([ReturnPeriodId]) REFERENCES [dbo].[ReturnPeriod] ([ReturnPeriodId])
);




GO



GO


GO


