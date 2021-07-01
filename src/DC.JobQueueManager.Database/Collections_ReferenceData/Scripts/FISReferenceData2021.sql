
DECLARE @CollectionId_FISReferenceData2021 INT = 175

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_FISReferenceData2021 AS CollectionId,N'FISReferenceData2021' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, 2021 AS CollectionYear, N'FIS Reference Data 2021' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'reference-data' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_FISReferenceData2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'DesktopReferenceData' As TaskName,1 As TaskOrder
 UNION
 SELECT @CollectionId_FISReferenceData2021 As CollectionId,N'ReferenceDataRetrieval' As SubscriptionName,N'ilr2021submissiontopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'DesktopReferenceDataPublish' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey
INSERT INTO @TempJobMessageKey
SELECT @CollectionId_FISReferenceData2021 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage