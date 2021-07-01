
DECLARE @CollectionId_ALLF INT = 120

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_ALLF AS CollectionId,N'ALLF' AS Name, 1 AS IsOpen, N'Generic' AS CollectionType, null AS CollectionYear, N'ALLF processing' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'allf-files' AS StorageReference,N'^(?i)(ALLF)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))((\.CSV))$' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_ALLF As CollectionId,55 as [PeriodNumber] , CONVERT(DATETIME, N'2020-02-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-03-17T06:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,56 as [PeriodNumber] , CONVERT(DATETIME, N'2020-03-18T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-04-20T06:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,57 as [PeriodNumber] , CONVERT(DATETIME, N'2020-04-21T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-05-19T06:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,58 as [PeriodNumber] , CONVERT(DATETIME, N'2020-05-20T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-06-16T06:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,59 as [PeriodNumber] , CONVERT(DATETIME, N'2020-06-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-07-16T06:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,60 as [PeriodNumber] , CONVERT(DATETIME, N'2020-07-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-18T08:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,61 as [PeriodNumber] , CONVERT(DATETIME, N'2020-08-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-09-16T08:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,62 as [PeriodNumber] , CONVERT(DATETIME, N'2020-09-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-10-16T08:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,63 as [PeriodNumber] , CONVERT(DATETIME, N'2020-10-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-11-17T09:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,64 as [PeriodNumber] , CONVERT(DATETIME, N'2020-11-18T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-12-16T09:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,65 as [PeriodNumber] , CONVERT(DATETIME, N'2020-12-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-01-19T09:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,66 as [PeriodNumber] , CONVERT(DATETIME, N'2021-01-20T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-02-16T09:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,67 as [PeriodNumber] , CONVERT(DATETIME, N'2021-02-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-03-16T09:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,68 as [PeriodNumber] , CONVERT(DATETIME, N'2021-03-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-04-20T08:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,69 as [PeriodNumber] , CONVERT(DATETIME, N'2021-04-21T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-05-19T07:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,70 as [PeriodNumber] , CONVERT(DATETIME, N'2021-05-20T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-06-16T07:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,71 as [PeriodNumber] , CONVERT(DATETIME, N'2021-06-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-07-16T07:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,72 as [PeriodNumber] , CONVERT(DATETIME, N'2021-07-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-17T07:00:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_ALLF As CollectionId,73 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-18T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-16T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,74 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-17T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-10-18T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,75 as [PeriodNumber] , CONVERT(DATETIME, N'2021-10-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-11-16T08:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,76 as [PeriodNumber] , CONVERT(DATETIME, N'2021-11-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-12-16T08:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,77 as [PeriodNumber] , CONVERT(DATETIME, N'2021-12-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-01-19T08:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,78 as [PeriodNumber] , CONVERT(DATETIME, N'2022-01-20T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-02-16T08:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,79 as [PeriodNumber] , CONVERT(DATETIME, N'2022-02-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-03-16T08:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,80 as [PeriodNumber] , CONVERT(DATETIME, N'2022-03-17T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-04-20T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,81 as [PeriodNumber] , CONVERT(DATETIME, N'2022-04-21T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-05-18T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,82 as [PeriodNumber] , CONVERT(DATETIME, N'2022-05-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-06-20T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,83 as [PeriodNumber] , CONVERT(DATETIME, N'2022-06-21T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-07-18T07:00:00') AS [EndDateTimeUtc]
UNION  SELECT @CollectionId_ALLF As CollectionId,84 as [PeriodNumber] , CONVERT(DATETIME, N'2022-07-19T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-08-16T07:00:00') AS [EndDateTimeUtc]

----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_ALLF As CollectionId,N'Process' As SubscriptionName,N'genericcollectionsubmissiontopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'ALLF' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_ALLF As CollectionId,N'CollectionReturnCodeALLF' As MessageKey,null As IsFirstStage

----------------------------------------------------------------------------------------------------------------------------------------


