
DECLARE @CollectionId_ILRValidationMessages1920 INT = 161

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_ILRValidationMessages1920 AS CollectionId,N'ILRValidationMessages1920' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, 1920 AS CollectionYear, N'ILR Validation Messages Reference Data Return period {periodNumber}' AS Description, N'2019 to 2020' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'reference-data' AS StorageReference,N'^(?i)(ILR1920ValidationMessages)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31)([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ILRValidationMessages1920 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'ILR1920ValidationMessages' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


