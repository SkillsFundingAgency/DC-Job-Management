DECLARE @CollectionId_PE_ALLF_Summarisation INT = 121

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_ALLF_Summarisation AS CollectionId,N'PE-ALLF-Summarisation' AS Name, 1 AS IsOpen, N'PE-ALLF' AS CollectionType, null AS CollectionYear, N'ALLF Summarisation' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_ALLF_Summarisation As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Generic' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_ALLF_Summarisation As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PublishtoBAU' As TaskName,2 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_ALLF_Summarisation As CollectionId,N'ProcessTypeGC' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_ALLF_Summarisation As CollectionId,N'CollectionTypeGC' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_ALLF_Summarisation As CollectionId,N'CollectionReturnCodeGC' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------