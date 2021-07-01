
DECLARE @CollectionId_1920_Final INT = 174

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_1920_Final AS CollectionId,N'1920-Final' AS Name, 1 AS IsOpen, N'FC' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_1920_Final As CollectionId,N'd8bcceb5-5628-4b8d-87cd-8c3d2e27b064' As TemplateOpenPeriod,null As TemplateClosePeriod,8 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


