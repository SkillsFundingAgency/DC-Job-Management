
DECLARE @CollectionId_PE_DAS_PeriodEndReport1819 INT = 50

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_PeriodEndReport1819 AS CollectionId,N'PE-DAS-PeriodEndReport1819' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1819 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'ilr1819-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


----ADD Topics/Tasks

--INSERT INTO @TempTopicTasks

------------------------------------------------------------------------------------------------------------------------------------------


