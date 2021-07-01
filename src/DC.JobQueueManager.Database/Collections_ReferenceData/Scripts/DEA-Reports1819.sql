
DECLARE @CollectionId_DEA_Reports1819 INT = 168

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_DEA_Reports1819 AS CollectionId,N'DEA-Reports1819' AS Name, 1 AS IsOpen, N'Publication' AS CollectionType, 1819 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'dea1819-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_DEA_Reports1819 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Publish' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_DEA_Reports1819 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_DEA_Reports1819 As CollectionId,N'ReportsPublicationSourceFolderAndContainer' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_DEA_Reports1819 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


