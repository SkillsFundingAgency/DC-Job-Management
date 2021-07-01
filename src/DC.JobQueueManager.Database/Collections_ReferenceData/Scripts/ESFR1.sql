
DECLARE @CollectionId_ESFR1 INT = 3

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob], [EmailOnJobCreation])
SELECT @CollectionId_ESFR1 AS CollectionId,N'ESFR1' AS Name, 1 AS IsOpen, N'ESF' AS CollectionType, 1819 AS CollectionYear, N'Round 1' AS Description, N'For data before 1 April 2019' AS SubText, 0 AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'esf-files' AS StorageReference,N'^(?i)(SUPPDATA)-([1-9][0-9]{7})-([0-9a-zA-Z-]{1,20})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_ESFR1 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2018-11-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-12-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2018-08-23T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-09-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2019-02-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-03-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2019-05-08T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-06-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2018-09-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-10-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,13 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-09-13T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2018-12-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-01-07T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2019-03-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-04-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2019-06-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-07-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,14 as [PeriodNumber] , CONVERT(DATETIME, N'2019-09-14T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-10-17T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2019-01-08T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-02-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2018-10-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-11-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2019-04-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-05-07T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR1 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2019-07-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-08-06T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ESFR1 As CollectionId,N'ESFV1' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ESFR1 As CollectionId,N'ESFV1' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Storage' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ESFR1 As CollectionId,N'ESFV1' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reports' As TaskName,3 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_ESFR1 As CollectionId,N'78237b46-7602-454f-9c3d-ec2601554909' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_ESFR1 As CollectionId,N'61c0129b-f03d-4e7f-b188-4dc59293cace' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


