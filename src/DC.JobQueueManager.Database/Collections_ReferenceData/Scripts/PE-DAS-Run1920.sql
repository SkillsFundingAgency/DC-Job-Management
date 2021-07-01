
DECLARE @CollectionId_PE_DAS_Run1920 INT = 71

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_Run1920 AS CollectionId,N'PE-DAS-Run1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DAS_Run1920 As CollectionId,N'Payments' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PeriodEndRun' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------
