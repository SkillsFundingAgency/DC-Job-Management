
DECLARE @CollectionId_PE_NCS_Summarisation2021 INT = 150

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_NCS_Summarisation2021 AS CollectionId,N'PE-NCS-Summarisation2021' AS Name, 1 AS IsOpen, N'PE-NCS' AS CollectionType, 2021 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ncs2021-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N're-run' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'NCS2021_C' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PublishtoBAU' As TaskName,3 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'CollectionTypeNCS' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'CollectionReturnCodeNCS' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_NCS_Summarisation2021 As CollectionId,N'ProcessTypeNCS' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------
