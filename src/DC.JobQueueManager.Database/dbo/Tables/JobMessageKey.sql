CREATE TABLE [dbo].[JobMessageKey] (
    [Id]           INT NOT NULL IDENTITY,
    [CollectionId] INT           NULL,
    [MessageKey]   VARCHAR (100) NOT NULL,
    [IsFirstStage] BIT NULL, 
    CONSTRAINT [PK_JobTopicMessageKey] PRIMARY KEY CLUSTERED ([Id] ASC)
);

