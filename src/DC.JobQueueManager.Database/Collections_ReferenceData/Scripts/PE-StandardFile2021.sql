
DECLARE @CollectionId_PE_StandardFile2021 INT = 192

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_StandardFile2021 AS CollectionId,N'PE-StandardFile2021' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2021 AS CollectionYear, N'D&MI - LIVE - 2021 - ILR - Freeze Extract - DCSS' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_StandardFile2021 As CollectionId,N'DataScience' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskSendData' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_StandardFile2021 As CollectionId,N'SqlJobName' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


