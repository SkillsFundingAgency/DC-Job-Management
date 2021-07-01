
DECLARE @CollectionId_PE_MCA_Reports2122 INT = 2122171

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_MCA_Reports2122 AS CollectionId,N'PE-MCA-Reports2122' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 2122 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend2122-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'MCAGLAReporting2122' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedOccupancyReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'MCAGLAReporting2122' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedFundingSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'MCAGLAReporting2122' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedNonContractedReport' As TaskName,3 As TaskOrder
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'MCAGLAReporting2122' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDestinationAndProgressionReport' As TaskName,4 As TaskOrder
----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'PreviousPeriodFileReference' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'McaGlaShortCode' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'AcademicYearStart' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_MCA_Reports2122 As CollectionId,N'AcademicYearEnd' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

