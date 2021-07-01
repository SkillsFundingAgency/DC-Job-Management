
DECLARE @CollectionId_2021_YearEnd INT = 2021140

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob], [EmailOnJobCreation])
SELECT @CollectionId_2021_YearEnd AS CollectionId,N'2021-YearEnd' AS Name, 1 AS IsOpen, N'FC' AS CollectionType, 2021 AS CollectionYear, N'Year-End (R10) Funding Claim 2020 to 2021' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, null AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob, 0 As EmailOnJobCreation

----------------------------------------------------------------------------------------------------------------------------------------

--ADD JobEmailTemplate
INSERT INTO @TempJobEmailTemplate
SELECT @CollectionId_2021_YearEnd As CollectionId,N'15ce580a-43f2-4e9a-b2d7-6050a1d5f96c' As TemplateOpenPeriod,null As TemplateClosePeriod,8 As JobStatus,1 As Active

----------------------------------------------------------------------------------------------------------------------------------------


