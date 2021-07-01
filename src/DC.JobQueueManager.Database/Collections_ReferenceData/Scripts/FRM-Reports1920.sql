
DECLARE @CollectionId_FRM_Reports1920 INT = 95

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_FRM_Reports1920 AS CollectionId,N'FRM-Reports1920' AS Name, 1 AS IsOpen, N'Publication' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'frm1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_FRM_Reports1920 As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2020-06-16T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-30T08:00:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_FRM_Reports1920 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,1 As IsFirstStage,1 As TopicEnabled,N'Validation' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_FRM_Reports1920 As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'Publish' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_FRM_Reports1920 As CollectionId,N'PauseWhenFinished' As MessageKey,1 As IsFirstStage
UNION  SELECT @CollectionId_FRM_Reports1920 As CollectionId,N'ReportsPublicationSourceFolderAndContainer' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


