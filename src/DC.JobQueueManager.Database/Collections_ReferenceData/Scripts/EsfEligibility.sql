
DECLARE @CollectionId_EsfEligibility INT = 199

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_EsfEligibility AS CollectionId,N'EsfEligibility' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, N'EsfEligibility Reference Data' AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'reference-data' AS StorageReference,null AS FileNameRegex, 0 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_EsfEligibility As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'TaskEsfEligibilityPublishStaging' As TaskName,1 As TaskOrder
UNION
 SELECT @CollectionId_EsfEligibility As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskEsfEligibilityPublishDB' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

--ADD JobMessageKey
INSERT INTO @TempJobMessageKey
SELECT @CollectionId_EsfEligibility As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage