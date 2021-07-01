
DECLARE @CollectionId_PE_UYP_LLVReport2122 INT = 2122179

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_UYP_LLVReport2122 AS CollectionId,N'PE-UYP-LLVReport2122' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2122 AS CollectionYear, N'Understand Your Payments -Learner Level View' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend2122-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_UYP_LLVReport2122 As CollectionId,N'Reports2122' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskLearnerLevelViewReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------



