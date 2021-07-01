
DECLARE @CollectionId_NCS1920 INT = 5

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_NCS1920 AS CollectionId,N'NCS1920' AS Name, 1 AS IsOpen, N'NCS' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ncs1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_NCS1920 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2020-01-09T04:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-02-07T06:16:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS1920 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2020-02-10T04:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-03-06T06:16:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS1920 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2020-03-09T04:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-04-07T03:00:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_NCS1920 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Funding' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,2 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_NCS1920 As CollectionId,N'TouchpointId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'ExternalJobId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'ExternalTimestamp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'ReportEndDate' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'DssContainer' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS1920 As CollectionId,N'ReportFileName' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


