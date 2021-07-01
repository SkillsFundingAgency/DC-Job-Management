
DECLARE @CollectionId_LarsPublish INT = 193

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_LarsPublish AS CollectionId,N'LARSPublish' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, null AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, 'reference-data' AS StorageReference, null AS FileNameRegex, 0 AS ResubmitJob

 ----------------------------------------------------------------------------------------------------------------------------------------

--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
SELECT @CollectionId_LarsPublish As CollectionId, N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskLarsPublishDB' As TaskName, 1 As TaskOrder
UNION 
SELECT @CollectionId_LarsPublish As CollectionId, N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskLarsPublishStorage' As TaskName, 2 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
SELECT @CollectionId_LarsPublish As CollectionId, N'MustUpdateLarsSearchIndex' As MessageKey, null As IsFirstStage
UNION
SELECT @CollectionId_LarsPublish As CollectionId, N'HasLarsDataChanged' As MessageKey, null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------