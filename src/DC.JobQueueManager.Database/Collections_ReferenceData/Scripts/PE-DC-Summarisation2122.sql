
DECLARE @CollectionId_PE_DC_Summarisation2122 INT = 2122155

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DC_Summarisation2122 AS CollectionId,N'PE-DC-Summarisation2122' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2122 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Main2122_FM35' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Main2122_FM25' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Main2122_ALB' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Main2122_TBL' As TaskName,4 As TaskOrder
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Main2122_EAS' As TaskName,5 As TaskOrder
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PublishToBAU' As TaskName,6 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'ProcessTypeDC' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'CollectionReturnCodeDC' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_DC_Summarisation2122 As CollectionId,N'CollectionTypeDC' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------