
DECLARE @CollectionId_OP_ILRProvidersReturningFirstTimePerDay_Report1920 INT = 170

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_OP_ILRProvidersReturningFirstTimePerDay_Report1920 AS CollectionId,N'OP-ILRProvidersReturningFirstTimePerDay-Report1920' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, 1920 AS CollectionYear, N'ILR Providers Returning First Time Per Day Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_ILRProvidersReturningFirstTimePerDay_Report1920 As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateILRReturningProvidersReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


