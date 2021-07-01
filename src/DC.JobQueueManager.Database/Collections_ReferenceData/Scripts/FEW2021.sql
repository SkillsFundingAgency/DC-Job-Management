
DECLARE @CollectionId_FEW2021 INT = 200

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_FEW2021 AS CollectionId,N'FEW2021' AS Name, 1 AS IsOpen, N'FEW' AS CollectionType, 2021 AS CollectionYear, N'Return period {periodNumber} - 2020 to 2021' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'few2021-files' AS StorageReference,N'^(?i)(FEW)-([1-9][0-9]{7})-([0-9]{4})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-([0-9]{2})(\.xml)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------

--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_FEW2021 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2021-07-01T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-31T17:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_FEW2021 As CollectionId,N'Process' As SubscriptionName,N'few2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_FEW2021 As CollectionId,N'Process' As SubscriptionName,N'few2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Storage' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_FEW2021 As CollectionId,N'Process' As SubscriptionName,N'few2021submissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Reporting' As TaskName,3 As TaskOrder


----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_FEW2021 As CollectionId,N'71861a10-eed9-49bb-a09d-e745cbcfdc01' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_FEW2021 As CollectionId,N'84e6ebd5-0986-44ab-aee5-7f4f1631df72' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

--ADD CollectionRelatedLink

INSERT INTO @TempCollectionRelatedLink
 SELECT @CollectionId_FEW2021 As CollectionId,N'Further education (FE) workforce staff record specification: 2020 to 2021' As Title,N'https://www.gov.uk/government/publications/further-education-workforce-data-collection' As Url,1 As SortOrder

----------------------------------------------------------------------------------------------------------------------------------------


