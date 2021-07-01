CREATE TABLE [dbo].[Organisation] (
    [OrganisationId] INT           NOT NULL IDENTITY,
    [Ukprn]          BIGINT        NOT NULL,
    [IsMCA] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([OrganisationId] ASC)
);
GO

