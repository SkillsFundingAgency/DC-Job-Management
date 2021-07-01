
DECLARE @CollectionId_1819_Final INT = 10

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_1819_Final AS CollectionId,N'1819-Final' AS Name, 1 AS IsOpen, N'FC' AS CollectionType, 1819 AS CollectionYear, N'Final funding claim 2018/19' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_1819_Final As CollectionId,N'e32112e0-dcea-44ec-9e9b-3b8d03b958a0' As TemplateOpenPeriod,null As TemplateClosePeriod,8 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


