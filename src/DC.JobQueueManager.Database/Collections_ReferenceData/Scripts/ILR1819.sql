
DECLARE @CollectionId_ILR1819 INT = 1

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_ILR1819 AS CollectionId,N'ILR1819' AS Name, 1 AS IsOpen, N'ILR' AS CollectionType, 1819 AS CollectionYear, N'Return period {periodNumber} - 2018 to 2019' AS Description, null AS SubText, 0 AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'ilr1819-files' AS StorageReference,N'^(?i)(ILR)-([1-9][0-9]{7})-([0-9]{4})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-([0-9]{2})((\.xml)|(\.zip))$' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_ILR1819 As CollectionId,14 as [PeriodNumber] , CONVERT(DATETIME, N'2019-09-17T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-10-05T02:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2018-08-23T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-09-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2018-09-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-10-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2018-10-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-11-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2018-11-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2018-12-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2018-12-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-01-07T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2019-01-08T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-02-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2019-02-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-03-06T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2019-03-07T06:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-04-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2019-04-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-05-07T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2019-05-08T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-06-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2019-06-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-07-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2019-07-05T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-08-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR1819 As CollectionId,13 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-09-13T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'ALB' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM25' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM35' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM36' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM70' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Funding' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM81' As TaskName,7 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Deds' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PersistDataToDeds' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAllbOccupancyReport' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateMainOccupancyReport' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateMathsAndEnglishReport' As TaskName,6 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAppsIndicativeEarningsReport' As TaskName,7 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateTrailblazerEmployerIncentivesReport' As TaskName,8 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingClaim1619Report' As TaskName,9 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateTrailblazerAppsOccupancyReport' As TaskName,10 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateHNSReport' As TaskName,11 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAdultFundingClaimReport' As TaskName,12 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateNonContractedAppsActivityReport' As TaskName,14 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateHNSSummaryReport' As TaskName,15 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateCommunityLearningReport' As TaskName,16 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateCLPOccupancyReport' As TaskName,17 As TaskOrder
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'Reports' As SubscriptionName,N'ilr1819submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateSummaryOfFM35FundingReport' As TaskName,18 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_ILR1819 As CollectionId,N'3cfbfb6b-0a8e-48f1-b716-268af491696b' As TemplateOpenPeriod,N'90a341c2-dcf2-41b7-87c7-4e341f02616d' As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'e2219426-4cd8-4bb6-9f96-f77ea040699a' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_ILR1819 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'InvalidLearnRefNumbers' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'ValidLearnRefNumbers' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'ValidationErrors' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'ValidationErrorLookups' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingAlbOutput' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingFm35Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingFm25Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingFm36Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingFm70Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FundingFm81Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'IlrReferenceData' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'OriginalFilename' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR1819 As CollectionId,N'FileSizeInBytes' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


