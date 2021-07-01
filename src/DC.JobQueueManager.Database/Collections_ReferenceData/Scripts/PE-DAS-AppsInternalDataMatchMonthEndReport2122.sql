
DECLARE @CollectionId_PE_DAS_AppsInternalDataMatchMonthEndReport2122 INT = 2122164

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_AppsInternalDataMatchMonthEndReport2122 AS CollectionId,N'PE-DAS-AppsInternalDataMatchMonthEndReport2122' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2122 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend2122-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DAS_AppsInternalDataMatchMonthEndReport2122 As CollectionId,N'Reports' As SubscriptionName,N'datamatchtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateInternalDataMatchReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_DAS_AppsInternalDataMatchMonthEndReport2122 As CollectionId,N'ILRPeriods' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------
