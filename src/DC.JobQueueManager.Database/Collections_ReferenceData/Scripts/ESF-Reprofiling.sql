
DECLARE @CollectionId_ESF_Reprofiling INT = 2021132

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_ESF_Reprofiling AS CollectionId,N'ESF-Reprofiling' AS Name, 1 AS IsOpen, N'ESFR' AS CollectionType, 2021 AS CollectionYear, 'ESF Reprofiling (ESFR)' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 1 AS MultiStageProcessing, N'dea1819-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
	  SELECT @CollectionId_ESF_Reprofiling As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2021-02-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-02-24T18:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,101 as [PeriodNumber] , CONVERT(DATETIME, N'2021-03-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-03-17T18:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2021-05-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-05-27T17:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,102 as [PeriodNumber] , CONVERT(DATETIME, N'2021-06-14T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-06-17T18:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,3 as [PeriodNumber] , CONVERT(DATETIME, N'2021-08-12T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-08-25T17:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,103 as [PeriodNumber] , CONVERT(DATETIME, N'2021-09-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-09-15T17:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,4 as [PeriodNumber] , CONVERT(DATETIME, N'2021-11-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-11-24T18:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,104 as [PeriodNumber] , CONVERT(DATETIME, N'2021-12-09T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2021-12-25T18:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,5 as [PeriodNumber] , CONVERT(DATETIME, N'2022-02-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-02-24T18:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,105 as [PeriodNumber] , CONVERT(DATETIME, N'2022-03-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-03-17T18:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,6 as [PeriodNumber] , CONVERT(DATETIME, N'2022-05-12T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-05-25T17:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,106 as [PeriodNumber] , CONVERT(DATETIME, N'2022-06-13T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-06-17T17:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,7 as [PeriodNumber] , CONVERT(DATETIME, N'2022-08-11T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-08-24T17:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,107 as [PeriodNumber] , CONVERT(DATETIME, N'2022-09-08T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-09-14T17:05:00') AS [EndDateTimeUtc] 

UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,8 as [PeriodNumber] , CONVERT(DATETIME, N'2022-11-11T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-11-24T18:05:00') AS [EndDateTimeUtc] 
UNION SELECT @CollectionId_ESF_Reprofiling As CollectionId,108 as [PeriodNumber] , CONVERT(DATETIME, N'2022-12-09T09:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2022-12-25T18:05:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------
