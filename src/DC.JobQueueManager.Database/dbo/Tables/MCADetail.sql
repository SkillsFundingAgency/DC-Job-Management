CREATE TABLE [dbo].[MCADetail] (
    [Id]           INT            NOT NULL IDENTITY,
    [Ukprn]                   BIGINT   NOT NULL UNIQUE,
    [GLACode]                 NVARCHAR(50)            NOT NULL,
    [SOFCode]       INT       NOT NULL, 
    [EffectiveFrom] DATETIME NOT NULL DEFAULT '2021-08-01', 
    [EffectiveTo] DATETIME NULL, 
    [AcademicYearFrom] INT NOT NULL DEFAULT 2122, 
    [AcademicYearTo] INT NULL, 
    CONSTRAINT [PK_OrganisationToGLACodeMapping] PRIMARY KEY ([Id])
);



