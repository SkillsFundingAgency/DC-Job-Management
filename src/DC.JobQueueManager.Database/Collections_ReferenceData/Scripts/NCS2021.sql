
DECLARE @CollectionId_NCS2021 INT = 110

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_NCS2021 AS CollectionId,N'NCS2021' AS Name, 1 AS IsOpen, N'NCS' AS CollectionType, 2021 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ncs2021-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_NCS2021 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2020-04-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-05-07T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2020-05-11T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-06-05T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2020-06-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-07-07T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2020-07-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-07T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2020-08-10T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-09-07T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2020-09-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-10-07T03:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2020-10-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-11-06T04:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2020-11-09T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-12-07T04:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2020-12-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-01-08T04:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2021-01-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-02-05T04:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2021-02-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-03-05T04:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_NCS2021 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2021-03-08T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-04-09T03:00:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_NCS2021 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Funding' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'Process' As SubscriptionName,N'ncssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,2 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_NCS2021 As CollectionId,N'ReportEndDate' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'ExternalTimestamp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'TouchpointId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'DssContainer' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'ExternalJobId' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_NCS2021 As CollectionId,N'ReportFileName' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


