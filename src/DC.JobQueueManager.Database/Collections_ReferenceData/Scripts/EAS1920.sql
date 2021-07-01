
DECLARE @CollectionId_EAS1920 INT = 4

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_EAS1920 AS CollectionId,N'EAS1920' AS Name, 1 AS IsOpen, N'EAS' AS CollectionType, 1920 AS CollectionYear, N'Return period {periodNumber}' AS Description, N'2019 to 2020' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'eas1920-files' AS StorageReference,N'^(?i)(EASDATA)-([1-9][0-9]{7})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob,1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_EAS1920 As CollectionId,13 as [PeriodNumber] , CONVERT(DATETIME, N'2020-08-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-09-14T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,14 as [PeriodNumber] , CONVERT(DATETIME, N'2020-09-25T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-10-22T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-22T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-09-05T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2019-09-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-10-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2019-10-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-11-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2019-11-18T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-12-05T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2019-12-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-01-07T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2020-01-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-02-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2020-02-18T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-03-05T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2020-03-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-04-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2020-04-20T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-05-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2020-05-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-06-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2020-06-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-07-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS1920 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2020-07-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-06T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_EAS1920 As CollectionId,N'Process' As SubscriptionName,N'eas1920submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'Process' As SubscriptionName,N'eas1920submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Storage' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'Process' As SubscriptionName,N'eas1920submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'Reports' As SubscriptionName,N'eas1920submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAdultFundingClaimReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'Reports' As SubscriptionName,N'eas1920submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'Reports' As SubscriptionName,N'eas1920submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedAdultEducationFundingSummaryReport' As TaskName,3 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_EAS1920 As CollectionId,N'0cff79db-9e77-4f67-aa37-5d10752751f3' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_EAS1920 As CollectionId,N'8ad7d223-fae9-4b37-a5e5-9aaa13fdc7c5' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


--ADD CollectionRelatedLink

INSERT INTO @TempCollectionRelatedLink
 SELECT @CollectionId_EAS1920 As CollectionId,N'Earnings adjustment statement (EAS) 2019 to 2020' As Title,N'https://www.gov.uk/government/publications/earnings-adjustment-statement-eas-2019-to-2020' As Url,1 As SortOrder

----------------------------------------------------------------------------------------------------------------------------------------


