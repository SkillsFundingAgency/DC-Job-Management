
DECLARE @CollectionId_COVIDRelief2 INT = 153

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_COVIDRelief2 AS CollectionId,N'COVIDRelief2' AS Name, 1 AS IsOpen, N'COVID2' AS CollectionType, 1920 AS CollectionYear, N'Return period {periodNumber}' AS Description, null AS SubText, 0 AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'covidrelief2-files' AS StorageReference,N'' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_COVIDRelief2 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2020-05-04T12:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-06-04T11:01:00') AS [EndDateTimeUtc] 
UNION  SELECT @CollectionId_COVIDRelief2 As CollectionId,2 as [PeriodNumber] , CONVERT(DATETIME, N'2020-06-22T08:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-07-03T11:01:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_COVIDRelief2 As CollectionId,N'ccaf7d8f-ac5d-4971-97f2-56940b3d7ca7' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active
UNION  SELECT @CollectionId_COVIDRelief2 As CollectionId,N'0719a3d9-feb6-4e33-a7a4-7a972819071f' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


