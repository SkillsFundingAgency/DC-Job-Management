
DECLARE @CollectionId_EAS2021 INT = 154

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_EAS2021 AS CollectionId,N'EAS2021' AS Name, 1 AS IsOpen, N'EAS' AS CollectionType, 2021 AS CollectionYear, N'Return period {periodNumber}' AS Description, N'2020 to 2021' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'eas2021-files' AS StorageReference,N'^(?i)(EASDATA)-([1-9][0-9]{7})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_EAS2021 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2020-08-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-09-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2020-09-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-10-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2020-10-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-11-05T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2020-11-13T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-12-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2020-12-14T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-01-07T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2021-01-15T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-02-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2021-02-12T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-03-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2021-03-12T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-04-08T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2021-04-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-05-07T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2021-05-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-06-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2021-06-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-07-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2021-07-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-05T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,13 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-13T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-14T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_EAS2021 As CollectionId,14 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-22T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-21T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_EAS2021 As CollectionId,N'Process' As SubscriptionName,N'eas2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'Process' As SubscriptionName,N'eas2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Storage' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'Process' As SubscriptionName,N'eas2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'Reports' As SubscriptionName,N'eas2021submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAdultFundingClaimReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'Reports' As SubscriptionName,N'eas2021submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'Reports' As SubscriptionName,N'eas2021submissiontopic' As TopicName,2 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedAdultEducationFundingSummaryReport' As TaskName,3 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_EAS2021 As CollectionId,N'0cff79db-9e77-4f67-aa37-5d10752751f3' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_EAS2021 As CollectionId,N'8ad7d223-fae9-4b37-a5e5-9aaa13fdc7c5' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


--ADD CollectionRelatedLink

INSERT INTO @TempCollectionRelatedLink
 SELECT @CollectionId_EAS2021 As CollectionId,N'Earnings adjustment statement (EAS) 2020 to 2021' As Title,N'https://www.gov.uk/government/publications/earnings-adjustment-statement-eas-2020-to-2021' As Url,1 As SortOrder

----------------------------------------------------------------------------------------------------------------------------------------


