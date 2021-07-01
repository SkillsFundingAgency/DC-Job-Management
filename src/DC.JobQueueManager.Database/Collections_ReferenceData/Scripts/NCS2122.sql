
DECLARE @CollectionId_NCS2122 INT = 195

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_NCS2122 AS CollectionId,N'NCS2122' AS Name, 1 AS IsOpen, N'NCS' AS CollectionType, 2122 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ncs2122-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
	   SELECT @CollectionId_NCS2122 As CollectionId,01 as [PeriodNumber] , CONVERT(DATETIME, N'2021-04-12T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-05-10T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,02 as [PeriodNumber] , CONVERT(DATETIME, N'2021-05-11T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-06-07T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,03 as [PeriodNumber] , CONVERT(DATETIME, N'2021-06-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-07-07T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,04 as [PeriodNumber] , CONVERT(DATETIME, N'2021-07-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-06T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,05 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-09T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-07T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,06 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-07T15:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,07 as [PeriodNumber] , CONVERT(DATETIME, N'2021-10-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-11-05T16:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,08 as [PeriodNumber] , CONVERT(DATETIME, N'2021-11-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-12-07T16:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,09 as [PeriodNumber] , CONVERT(DATETIME, N'2021-12-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-01-10T16:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2022-01-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-02-07T16:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2022-02-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-03-07T16:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2122 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2022-03-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-04-07T15:00:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_NCS2122 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Funding' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,2 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
	   SELECT @CollectionId_NCS2122 As CollectionId,N'ReportEndDate' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'ExternalTimestamp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'TouchpointId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'DssContainer' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'ExternalJobId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2122 As CollectionId,N'ReportFileName' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

