
DECLARE @CollectionId_PE_App_Summarisation1920 INT = 56

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_App_Summarisation1920 AS CollectionId,N'PE-App-Summarisation1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Apps1920_Levy' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Apps1920_NonLevy' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Apps1920_EAS' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'Summarisation' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PublishToBAU' As TaskName,4 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'ProcessTypeApp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'CollectionTypeApp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_App_Summarisation1920 As CollectionId,N'CollectionReturnCodeApp' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


