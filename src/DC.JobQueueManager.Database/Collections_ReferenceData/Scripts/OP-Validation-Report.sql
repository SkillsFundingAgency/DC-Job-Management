
DECLARE @CollectionId_OP_Validation_Report INT = 98

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_OP_Validation_Report AS CollectionId,N'OP-Validation-Report' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, null AS CollectionYear, N'Validation Rule Details Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_Validation_Report As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateValidationRuleDetailReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_OP_Validation_Report As CollectionId,N'SelectedILRPeriods' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_OP_Validation_Report As CollectionId,N'SelectedCollectionYear' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_OP_Validation_Report As CollectionId,N'Rule' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


