
DECLARE @CollectionId_OP_ILRFileSubmissionsPerDay_Report2021 INT = 173

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_OP_ILRFileSubmissionsPerDay_Report2021 AS CollectionId,N'OP-ILRFileSubmissionsPerDay-Report2021' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, 2021 AS CollectionYear, N'ILR Submissions Per Day Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_ILRFileSubmissionsPerDay_Report2021 As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateILRFileSubmissionsPerDayReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


