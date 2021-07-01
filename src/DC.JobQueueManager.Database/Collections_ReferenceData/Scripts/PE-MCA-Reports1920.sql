
DECLARE @CollectionId_PE_MCA_Reports1920 INT = 82

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_MCA_Reports1920 AS CollectionId,N'PE-MCA-Reports1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-22T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2019-10-01T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,N'MCAGLAReporting1920' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedOccupancyReport' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,N'MCAGLAReporting1920' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedFundingSummaryReport' As TaskName,2 As TaskOrder
UNION  SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,N'MCAGLAReporting1920' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateDevolvedNonContractedReport' As TaskName,3 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,N'McaGlaShortCode' As MessageKey,null As IsFirstStage
UNION  SELECT @CollectionId_PE_MCA_Reports1920 As CollectionId,N'PreviousPeriodFileReference' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------

