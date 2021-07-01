﻿CREATE TABLE [dbo].[Migration] (
    [MigrationId]           UNIQUEIDENTIFIER NOT NULL,
    [Description]           NVARCHAR (500)   NOT NULL,
    [DateTimeCreatedUTC]    DATETIME2		 NOT NULL DEFAULT (GETUTCDATE()),
    [Author]				NVARCHAR (200)   NOT NULL,
    [BUILD_BUILDNUMBER]		NVARCHAR (150)   NOT NULL,
    [BUILD_BRANCHNAME]		NVARCHAR (150)   NOT NULL,
    [RELEASE_RELEASENAME]	NVARCHAR (150)   NOT NULL,
    CONSTRAINT [PK_Migration] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
);