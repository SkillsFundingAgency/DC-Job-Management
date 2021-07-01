DECLARE @TempCollection TABLE (
    [CollectionId]              INT             NOT NULL,
    [Name]                      VARCHAR (60)    NOT NULL,
    [IsOpen]                    BIT             NOT NULL,
    [CollectionType]            VARCHAR(100)    NOT NULL,
    [CollectionYear]            INT,
    [Description]               VARCHAR (250)   NULL,
    [SubText]                   VARCHAR (500)   NULL,
    [CrossloadingEnabled]       BIT             NULL,
    [ProcessingOverrideFlag]    BIT             NULL,
    [MultiStageProcessing]      BIT,
    [StorageReference]          NVARCHAR (100)  NULL,
    [FileNameRegex]             NVARCHAR(MAX)   NULL,
    [ResubmitJob]               BIT             NOT NULL,
    [EmailOnJobCreation]        BIT             NOT NULL DEFAULT 0
);


DECLARE @TempReturnPeriod TABLE (
    [CollectionId]     INT          ,
    [PeriodNumber]     INT			,
   [StartDateTimeUTC] DATETIME     ,
    [EndDateTimeUTC]   DATETIME    
);


DECLARE @TempCollectionRelatedLink TABLE (
	[CollectionId] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[SortOrder] [int] NOT NULL
);

DECLARE @TempTopicTasks TABLE (
    CollectionId INT,
    [SubscriptionName] nvarchar(100), 
    TopicName nvarchar(100), 
    TopicOrder int, 
    IsFirstStage bit, 
    [TopicEnabled] bit, 
    TaskName varchar(100), 
    TaskOrder int
);


DECLARE @TempJobMessageKey TABLE (
    [CollectionId] INT           ,
    [MessageKey]   VARCHAR (100),
    [IsFirstStage] BIT 
);


DECLARE @TempJobEmailTemplate TABLE(
	[CollectionId]		    INT	 NOT NULL,
	[TemplateOpenPeriod]	VARCHAR(500) NOT NULL,
	[TemplateClosePeriod]	VARCHAR(500) NULL , 
	[JobStatus]				SMALLINT	 NOT NULL,
	[Active]				BIT			 NOT NULL DEFAULT 1
);



DECLARE @TempSchedule TABLE (
    [ID]                    INT,
    [CollectionId]          INT		 ,
    [JobTitle]	            NVARCHAR(100),
    [Enabled]				BIT      ,
    [MinuteIsCadence]		BIT      ,
    [Minute]				TINYINT  ,
    [Hour]					TINYINT  ,
    [DayOfTheMonth]			TINYINT  ,
    [Month]					TINYINT  ,
    [DayOfTheWeek]			TINYINT	 ,
    [ExecuteOnceOnly]		BIT      ,
    [LastExecuteDateTime]	DATETIME ,
	[Paused]				BIT		 
);
