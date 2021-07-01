
DECLARE @CollectionId_PE_Data_Warehouse12021 INT = 190

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_Data_Warehouse12021 AS CollectionId,N'PE-Data-Warehouse12021' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2021 AS CollectionYear, N'Apprenticeship Service - Monthly SN Snapshot' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_Data_Warehouse12021 As CollectionId,N'DataScience' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskSendData' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_Data_Warehouse12021 As CollectionId,N'SqlJobName' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

