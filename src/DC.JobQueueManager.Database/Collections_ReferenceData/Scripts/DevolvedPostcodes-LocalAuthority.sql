
DECLARE @CollectionId_DevolvedPostcodes_LocalAuthority INT = 163

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_DevolvedPostcodes_LocalAuthority AS CollectionId,N'DevolvedPostcodes-LocalAuthority' AS Name, 1 AS IsOpen, N'REF' AS CollectionType, null AS CollectionYear, N'Devolved Postcodes - LocalAuthority Reference Data' AS Description, N'' AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'reference-data' AS StorageReference,N'^(?i)(MCAGLA_LocalAuthority_RD)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31)([01][0-9]|2[0-3])([0-5][0-9]))(\.csv)$' AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_DevolvedPostcodes_LocalAuthority As CollectionId,N'Process' As SubscriptionName,N'referencedatatopic' As TopicName,1 As TopicOrder,null As IsFirstStage,1 As TopicEnabled,N'DevolvedPostcodesLocalAuthority' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


