
DECLARE @CollectionId_FISReferenceData2122 INT = 204
DECLARE @TopicName_ILR2122_FISRD varchar(22) = 'ilr2122submissiontopic';

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob], [EmailOnJobCreation])
SELECT @CollectionId_FISReferenceData2122 AS CollectionId,N'FISReferenceData2122' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, 2122 AS CollectionYear, N'FIS Reference Data 2122' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'reference-data' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob, 0 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_FISReferenceData2122 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,@TopicName_ILR2122_FISRD As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'DesktopReferenceData' As TaskName,1 As TaskOrder
 UNION
 SELECT @CollectionId_FISReferenceData2122 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,@TopicName_ILR2122_FISRD As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'DesktopReferenceDataPublish' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey
INSERT INTO @TempJobMessageKey
SELECT @CollectionId_FISReferenceData2122 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage