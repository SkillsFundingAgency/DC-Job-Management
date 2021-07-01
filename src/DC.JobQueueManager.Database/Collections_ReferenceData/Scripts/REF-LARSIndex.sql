
DECLARE @CollectionId_FALASearchIndexRebuild INT = 194

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_FALASearchIndexRebuild AS CollectionId,N'FALASearchIndexRebuild' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, null AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, 'reference-data' AS StorageReference, null AS FileNameRegex, 0 AS ResubmitJob

 ----------------------------------------------------------------------------------------------------------------------------------------

--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
SELECT @CollectionId_FALASearchIndexRebuild As CollectionId, N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'TaskLarsPublishSearch' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
SELECT @CollectionId_FALASearchIndexRebuild As CollectionId, N'MustUpdateLarsSearchIndex' As MessageKey, null As IsFirstStage
UNION
SELECT @CollectionId_FALASearchIndexRebuild As CollectionId, N'HasLarsDataChanged' As MessageKey, null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------