
DECLARE @CollectionId_REF_EPA INT = 20

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_REF_EPA AS CollectionId,N'REF-EPA' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_REF_EPA As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'Epa' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Schedule

INSERT INTO @TempSchedule
 SELECT 201 As ID, @CollectionId_REF_EPA As CollectionId,N'REF-EPA' As JobTitle,1 As Enabled,0 As MinuteIsCadence,16 As Minute,6 As Hour,null As DayOfTheMonth,null As Month,null As DayOfTheWeek,0 As ExecuteOnceOnly,CONVERT(DATETIME, N'2020-10-09T06:16:00') As LastExecuteDateTime,0 As Paused

----------------------------------------------------------------------------------------------------------------------------------------
