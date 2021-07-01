
DECLARE @CollectionId_DevolvedContracts INT = 169

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob],[EmailOnJobCreation])
SELECT @CollectionId_DevolvedContracts AS CollectionId,N'DevolvedContracts' AS Name, 1 AS IsOpen, N'MCA-GLA' AS CollectionType, null AS CollectionYear, N'Devolved Contracts Reference Data' AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'mca-gla' AS StorageReference,N'^(?i)[0-9]{8}-(DevolvedContracts)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31)([01][0-9]|2[0-3])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob, 1 AS EmailOnJobCreation


----------------------------------------------------------------------------------------------------------------------------------------

--ADD Return Periods

INSERT INTO @TempReturnPeriod
 SELECT @CollectionId_DevolvedContracts As CollectionId,1 as [PeriodNumber] , CONVERT(DATETIME, N'2019-01-01T00:00:00') AS [StartdateTimeUtc], CONVERT(DATETIME, N'2600-07-31T00:00:00') AS [EndDateTimeUtc] 

 ----------------------------------------------------------------------------------------------------------------------------------------

--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_DevolvedContracts As CollectionId,N'Process' As SubscriptionName,N'devolvedcontractssubmissiontopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'DevolvedContracts' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

--ADD JobMessageKey

INSERT INTO @TempJobMessageKey
 SELECT @CollectionId_DevolvedContracts As CollectionId,N'McaGlaShortCode' As MessageKey,null As IsFirstStage

 --ADD JobEmailTemplate

INSERT INTO @TempJobEmailTemplate
 SELECT @CollectionId_DevolvedContracts As CollectionId,N'f633fb2d-660d-4942-b329-87929c4c04ad' As TemplateOpenPeriod,null As TemplateClosePeriod,1 As JobStatus,1 As Active