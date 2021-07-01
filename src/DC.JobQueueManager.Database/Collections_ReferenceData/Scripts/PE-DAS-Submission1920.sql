
DECLARE @CollectionId_PE_DAS_Submission1920 INT = 73

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_Submission1920 AS CollectionId,N'PE-DAS-Submission1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ilr1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DAS_Submission1920 As CollectionId,N'GenerateFM36Payments' As SubscriptionName,N'ilr1920submissiontopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'ProcessPeriodEnd' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_DAS_Submission1920 As CollectionId,N'GenerateFM36Payments' As SubscriptionName,N'ilr1920submissiontopic' As TopicName,2 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'JobSuccess' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_DAS_Submission1920 As CollectionId,N'FundingFm36OutputPeriodEnd' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

