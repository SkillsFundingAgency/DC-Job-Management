
DECLARE @CollectionId_PE_DataExtract_Report2122 INT = 2122173

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DataExtract_Report2122 AS CollectionId,N'PE-DataExtract-Report2122' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2122 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend2122-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DataExtract_Report2122 As CollectionId,N'SummarisationReporting' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGeneratePeriodSummaryReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_DataExtract_Report2122 As CollectionId,N'SummarisationReporting' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGeneratePeriodSummaryReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_DataExtract_Report2122 As CollectionId,N'CollectionReturnCodeApp' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_DataExtract_Report2122 As CollectionId,N'CollectionReturnCodeDC' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_DataExtract_Report2122 As CollectionId,N'CollectionReturnCodeESF' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------
