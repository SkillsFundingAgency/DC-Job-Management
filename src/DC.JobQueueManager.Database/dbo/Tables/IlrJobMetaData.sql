CREATE TABLE [dbo].[IlrJobMetaData] (
    [Id]                   BIGINT   IDENTITY (1, 1) NOT NULL,
    [JobId]                BIGINT   NOT NULL,
    [DateTimeSubmittedUtc] DATETIME NOT NULL,
    CONSTRAINT [PK_IlrJobMetaData] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (PAD_INDEX = OFF, DATA_COMPRESSION = PAGE),
    CONSTRAINT [FK_IlrJobMetaData_IlrJobMetaData] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([JobId])
);
GO

CREATE NONCLUSTERED INDEX [IX_IlrMetaData_JobId] ON [dbo].[IlrJobMetaData] ([JobId] ASC) INCLUDE ([DateTimeSubmittedUtc]) 
GO