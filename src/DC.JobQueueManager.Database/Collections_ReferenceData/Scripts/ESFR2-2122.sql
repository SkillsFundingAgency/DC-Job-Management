
DECLARE @CollectionId_ESFR2_2122 INT = 205

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_ESFR2_2122 AS CollectionId,N'ESFR2-2122' AS Name, 1 AS IsOpen, N'ESF' AS CollectionType, 2122 AS CollectionYear, N'Round 2' AS Description, N'For data after 1 April 2019' AS SubText, 0 AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'esf-files' AS StorageReference,N'^(?i)(SUPPDATA2)-([1-9][0-9]{7})-([0-9a-zA-Z-]{1,20})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_ESFR2_2122 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-06T17:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-06T17:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2021-10-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-11-04T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2021-11-12T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-12-06T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2021-12-14T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-01-07T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2022-01-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-02-04T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2022-02-14T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-03-04T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2022-03-14T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-04-06T18:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2022-04-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-05-06T17:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2022-05-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-06-08T17:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2022-06-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-07-06T17:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2022-07-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-08-04T17:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ESFR2_2122 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Storage' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingReport' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateEsfAimAndDeliverableReport' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,5 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_ESFR2_2122 As CollectionId,N'61c0129b-f03d-4e7f-b188-4dc59293cace' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active
UNION  SELECT @CollectionId_ESFR2_2122 As CollectionId,N'78237b46-7602-454f-9c3d-ec2601554909' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------