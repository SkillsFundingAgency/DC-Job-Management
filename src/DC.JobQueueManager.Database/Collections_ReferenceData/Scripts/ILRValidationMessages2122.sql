
DECLARE @CollectionId_ILRValidationMessages2122 INT = 203

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob], [EmailOnJobCreation])
SELECT @CollectionId_ILRValidationMessages2122 AS CollectionId,N'ILRValidationMessages2122' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, 2122 AS CollectionYear, N'ILR Validation Messages Reference Data Return period {periodNumber}' AS Description, N'2021 to 2022' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'reference-data' AS StorageReference,N'^(?i)(ILR2122ValidationMessages)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31)([01][0-9]|2[0-3])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 0 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ILRValidationMessages2122 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'ILR2122ValidationMessages' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


