
DECLARE @CollectionId_PE_DAS_Stop2021 INT = 202172

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_Stop2021 AS CollectionId,N'PE-DAS-Stop2021' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2021 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DAS_Stop2021 As CollectionId,N'Payments' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'PeriodEndStop' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

