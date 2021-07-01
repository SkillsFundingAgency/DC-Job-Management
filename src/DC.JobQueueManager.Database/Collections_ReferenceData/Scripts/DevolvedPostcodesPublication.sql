
DECLARE @CollectionId_DevolvedPostcodesPublication INT = 179

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_DevolvedPostcodesPublication AS CollectionId,N'DevolvedPostcodes-Publication' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, N'Devolved Postcodes Publication Reference Data' AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'reference-data' AS StorageReference,N'' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_DevolvedPostcodesPublication As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'DevolvedPostcodesPublication' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


