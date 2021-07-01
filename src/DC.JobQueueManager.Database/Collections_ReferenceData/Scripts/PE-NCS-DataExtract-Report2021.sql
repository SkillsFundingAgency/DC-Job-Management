
DECLARE @CollectionId_PE_NCS_DataExtract_Report2021 INT = 151

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_NCS_DataExtract_Report2021 AS CollectionId,N'PE-NCS-DataExtract-Report2021' AS Name, 1 AS IsOpen, N'PE-NCS' AS CollectionType, 2021 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend2021-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_NCS_DataExtract_Report2021 As CollectionId,N'SummarisationReporting' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGeneratePeriodSummaryReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_NCS_DataExtract_Report2021 As CollectionId,N'CollectionReturnCodeNCS' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------
