CREATE TABLE [dbo].[Collection] (
    [CollectionId]           INT            NOT NULL,
    [Name]                   VARCHAR (60)   NOT NULL,
    [IsOpen]                 BIT            NOT NULL,
    [CollectionTypeId]       INT            NOT NULL,
    [CollectionYear]         INT            CONSTRAINT [DF__Collectio__Colle__5070F446] DEFAULT ((1819)) NULL,
    [Description]            VARCHAR (250)  NULL,
    [SubText]                VARCHAR (500)  NULL,
    [CrossloadingEnabled]    BIT            NULL,
    [ProcessingOverrideFlag] BIT            NULL,
    [MultiStageProcessing]   BIT            CONSTRAINT [DF_Collection_EmailOnJobCreation] DEFAULT ((0)) NOT NULL,
    [StorageReference]       NVARCHAR (100) NULL,
    [FileNameRegex]          NVARCHAR(MAX)   NULL,
    [ResubmitJob]            BIT             NOT NULL DEFAULT(1),
    [EmailOnJobCreation]     BIT             NOT NULL DEFAULT(0)

    CONSTRAINT [PK_Collection] PRIMARY KEY CLUSTERED ([CollectionId] ASC),
    CONSTRAINT [FK_Collection_CollectionType] FOREIGN KEY ([CollectionTypeId]) REFERENCES [dbo].[CollectionType] ([CollectionTypeId])
);