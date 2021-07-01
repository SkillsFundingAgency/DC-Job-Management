
CREATE TABLE [dbo].[Job] (
    [JobId]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [CollectionId]         INT            NOT NULL,
    [Priority]             SMALLINT       NOT NULL,
    [DateTimeCreatedUTC]   DATETIME       NOT NULL,
    [DateTimeUpdatedUTC]   DATETIME       NULL,
    [CreatedBy]            VARCHAR (50)   NULL,
    [Status]               SMALLINT       NOT NULL,
    [RowVersion]           ROWVERSION     NULL,
    [NotifyEmail]          NVARCHAR (500) NULL,
    [CrossLoadingStatus]   SMALLINT       NULL,
    [Ukprn] BIGINT NULL, 
    CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED ([JobId] ASC) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE),
    CONSTRAINT [FK_Job_Collection] FOREIGN KEY ([CollectionId]) REFERENCES [dbo].[Collection] ([CollectionId])
);
GO
CREATE NONCLUSTERED INDEX [IDX_Job_CollectionId] ON [dbo].[Job] ([CollectionId]) INCLUDE ([Status],[Ukprn]) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE);
GO
CREATE NONCLUSTERED INDEX [IDX_Job_Status_UKPRN] ON [dbo].[Job] ([Status],[Ukprn]) INCLUDE ([CollectionId],[DateTimeCreatedUTC],[DateTimeUpdatedUTC]) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE);
GO
CREATE NONCLUSTERED INDEX [IDX_Job_CollectionId_Status] ON [dbo].[Job] ([CollectionId],[Status]) INCLUDE ([DateTimeCreatedUTC],[DateTimeUpdatedUTC],[Ukprn]) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE)
GO
