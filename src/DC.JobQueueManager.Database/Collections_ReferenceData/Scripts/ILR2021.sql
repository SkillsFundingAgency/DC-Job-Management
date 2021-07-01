
DECLARE @CollectionId_ILR2021 INT = 140

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_ILR2021 AS CollectionId,N'ILR2021' AS Name, 1 AS IsOpen, N'ILR' AS CollectionType, 2021 AS CollectionYear, N'Return period {periodNumber} - 2020 to 2021' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'ilr2021-files' AS StorageReference,N'^(?i)(ILR)-([1-9][0-9]{7})-([0-9]{4})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-([0-9]{2})((\.xml)|(\.zip))$' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_ILR2021 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2020-08-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-09-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2020-09-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-10-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2020-10-07T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-11-05T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2020-11-13T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-12-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2020-12-14T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-01-07T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2021-01-15T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-02-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2021-02-12T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-03-04T06:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2021-03-12T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-04-08T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,9 as [PeriodNumber] , CONVERT(DATETIME, N'2021-04-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-05-07T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2021-05-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-06-04T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2021-06-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-07-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2021-07-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-05T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,13 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-13T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-14T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ILR2021 As CollectionId,14 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-22T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-21T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ILR2021 As CollectionId,N'FileValidation' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'Process' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FileValidation' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Process' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,2 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'IlrMessage' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,2 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'IlrMessage' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,2 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'LearnerReferenceData' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,2 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FrmReferenceData' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,2 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'LearnerReferenceData' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Validation' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,3 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'Process' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Validation' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,3 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Process' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'ALB' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateRuleViolationSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM25' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReportV2' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM35' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM36' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM70' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Funding' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,4 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'FM81' As TaskName,6 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'GenerateFM36Payments' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,5 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Process' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PersistDataToDeds' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreInvalidTables' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreValidTables' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreALBTables' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreFM25Tables' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreFM35Tables' As TaskName,6 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreFM36Tables' As TaskName,7 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreFM70Tables' As TaskName,8 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreFM81Tables' As TaskName,9 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Deds' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,6 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskStoreValidationTables' As TaskName,10 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'GenerateFM36Payments' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,7 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'JobSuccess' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAllbOccupancyReport' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateMainOccupancyReport' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateMathsAndEnglishReport' As TaskName,6 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAppsIndicativeEarningsReport' As TaskName,7 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateTrailblazerEmployerIncentivesReport' As TaskName,8 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingClaim1619Report' As TaskName,9 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateTrailblazerAppsOccupancyReport' As TaskName,10 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateHNSReport' As TaskName,11 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAdultFundingClaimReport' As TaskName,12 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateNonContractedAppsActivityReport' As TaskName,14 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateHNSSummaryReport' As TaskName,15 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateCommunityLearningReport' As TaskName,16 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateSummaryOfFm35FundingReport' As TaskName,17 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedAdultEducationOccupancyReport' As TaskName,18 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedAdultEducationFundingSummaryReport' As TaskName,19 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateSummaryOfFundingByStudentReport' As TaskName,20 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateRuleViolationSummaryReport' As TaskName,21 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationReportV2' As TaskName,22 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateNonContractDevolvedAdultEducationOccupancyReport' As TaskName,23 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAEBSTFInitiativesOccupancyReport' As TaskName,24 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedAdultEducationOccupancyV2Report' As TaskName,25 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,8 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAEBSTFFundingSummaryReport ' As TaskName,26 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoringReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoring06Report' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoring07Report' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoring08Report' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoring09Report' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'frmsubmissiontopic' As TopicName,9 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingRulesMonitoring15Report' As TaskName,6 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,10 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateEsfAimAndDeliverableReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ESFV2' As SubscriptionName,N'esfsubmissiontopic' As TopicName,10 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'Reports' As SubscriptionName,N'datamatchtopic' As TopicName,11 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDataMatchReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_ILR2021 As CollectionId,N'3cfbfb6b-0a8e-48f1-b716-268af491696b' As TemplateOpenPeriod,N'90a341c2-dcf2-41b7-87c7-4e341f02616d' As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'e2219426-4cd8-4bb6-9f96-f77ea040699a' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


--ADD CollectionRelatedLink

INSERT INTO @TempCollectionRelatedLink
 SELECT @CollectionId_ILR2021 As CollectionId,N'ILR guides and templates for 2020 to 2021' As Title,N'https://guidance.submitlearnerdatabeta.fasst.org.uk/previous/20-21/psm' As Url,1 As SortOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ILR specification 2020 to 2021' As Title,N'https://guidance.submitlearnerdatabeta.fasst.org.uk/previous/20-21/ilr' As Url,2 As SortOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ILR validation rules and appendices 2020 to 2021' As Title,N'https://guidance.submitlearnerdatabeta.fasst.org.uk/previous/20-21/ilr/appendices' As Url,3 As SortOrder
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'How to provide and maintain accurate ILR data' As Title,N'https://www.gov.uk/government/collections/individualised-learner-record-ilr#how-to-provide-and-maintain-accurate-ilr-data' As Url,4 As SortOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_ILR2021 As CollectionId,N'FundingFm81Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FileSizeInBytes' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FundingFm35Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FundingAlbOutput' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'IlrReferenceData' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FrmReferenceData' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'InvalidLearnRefNumbers' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ValidationErrors' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ValidLearnRefNumbers' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FundingFm36Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FundingFm25Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'FundingFm70Output' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'OriginalFilename' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'ValidationErrorLookups' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_ILR2021 As CollectionId,N'LearnerReferenceData' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


