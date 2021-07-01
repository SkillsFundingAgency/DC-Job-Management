
DECLARE @CollectionId_REF_ULN INT = 22

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_REF_ULN AS CollectionId,N'REF-ULN' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_REF_ULN As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Uln' As TaskName,1 As TaskOrder
UNION  SELECT @CollectionId_REF_ULN As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'UlnLrs' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Schedule

INSERT INTO @TempSchedule
 SELECT 1 As ID, @CollectionId_REF_ULN As CollectionId,N'REF-ULN' As JobTitle,0 As Enabled,0 As MinuteIsCadence,15 As Minute,null As Hour,null As DayOfTheMonth,null As Month,null As DayOfTheWeek,0 As ExecuteOnceOnly,CONVERT(DATETIME, N'2020-07-08T09:15:00') As LastExecuteDateTime,0 As Paused

----------------------------------------------------------------------------------------------------------------------------------------


