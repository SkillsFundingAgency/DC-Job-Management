
DECLARE @CollectionId_COVID19 INT = 152

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_COVID19 AS CollectionId,N'COVID19' AS Name, 1 AS IsOpen, N'COVID' AS CollectionType, 1920 AS CollectionYear, N'Return period {periodNumber}' AS Description, null AS SubText, 0 AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'covid19-files' AS StorageReference,N'' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_COVID19 As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2020-04-24T12:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2020-04-30T11:01:00') AS [EndDateTimeUtc] 

----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_COVID19 As CollectionId,N'1757dd4d-f37f-454b-9538-4e86cdab43dd' As TemplateOpenPeriod,null As TemplateClosePeriod,4 As JobStatus,1 As Active
UNION  SELECT @CollectionId_COVID19 As CollectionId,N'0c738e55-ecfa-4fd9-a5a7-a12e540a3a57' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


