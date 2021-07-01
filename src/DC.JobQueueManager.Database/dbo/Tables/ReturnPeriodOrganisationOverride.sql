
CREATE TABLE [dbo].[ReturnPeriodOrganisationOverride] (
    [Id]             INT IDENTITY (1, 1) NOT NULL,
    [ReturnPeriodId] INT NOT NULL,
    [OrgaisationId]  INT NOT NULL,
    [PeriodNumber]   INT NOT NULL,
    CONSTRAINT [PK_ReturnPeriodOrganisationOverride] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReturnPeriodOrganisationOverride_Organisation] FOREIGN KEY ([OrgaisationId]) REFERENCES [dbo].[Organisation] ([OrganisationId]),
    CONSTRAINT [FK_ReturnPeriodOrganisationOverride_ReturnPeriod] FOREIGN KEY ([ReturnPeriodId]) REFERENCES [dbo].[ReturnPeriod] ([ReturnPeriodId])
);


GO


GO


GO


GO


GO
