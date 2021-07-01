
DECLARE @CollectionId_1819_YearEnd INT = 6

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_1819_YearEnd AS CollectionId,N'1819-YearEnd' AS Name, 1 AS IsOpen, N'FC' AS CollectionType, 1819 AS CollectionYear, N'Year end forecast 2018/19' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_1819_YearEnd As CollectionId,12 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_1819_YearEnd As CollectionId,11 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-06T05:05:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_1819_YearEnd As CollectionId,10 as [PeriodNumber] , CONVERT(DATETIME, N'2019-08-07T05:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-08-06T05:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


