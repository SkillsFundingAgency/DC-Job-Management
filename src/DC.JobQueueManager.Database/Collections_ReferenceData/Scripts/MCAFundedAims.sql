
DECLARE @CollectionId_FundedAims INT = 206

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_FundedAims AS CollectionId,N'MCA-GLA-FundedAims' AS Name, 1 AS IsOpen, N'MCA-GLA-FA' AS CollectionType, null AS CollectionYear, N'Funded aims for MCA/GLA ' AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'mca-gla-funded-aims-files' AS StorageReference,N'^(?i)[0-9]{8}-(FundedAims)-((19|20)[0-9]{2}[0-1]{1}[0-4]{1})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31)([01][0-9]|2[0-3])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------

--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_FundedAims As CollectionId,192014 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-13T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-01T17:00:00') AS [EndDateTimeUtc] 

 ----------------------------------------------------------------------------------------------------------------------------------------

--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_FundedAims As CollectionId,N'Process' As SubscriptionName,N'mcafundedaimstopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'FundedAims' As TaskName,1 As TaskOrder

