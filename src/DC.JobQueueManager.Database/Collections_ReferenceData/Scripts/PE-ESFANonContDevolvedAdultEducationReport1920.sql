
DECLARE @CollectionId_PE_ESFANonContDevolvedAdultEducationReport1920 INT = 130

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_ESFANonContDevolvedAdultEducationReport1920 AS CollectionId,N'PE-ESFANonContDevolvedAdultEducationReport1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, N'ESFA Non-Contracted Devolved Adult Education Activity Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_ESFANonContDevolvedAdultEducationReport1920 As CollectionId,N'MCAGLAReporting1920' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateEsfaNonContractedDevolvedAdultEducationActivityReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_ESFANonContDevolvedAdultEducationReport1920 As CollectionId,N'McaGlaShortCodeList' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

