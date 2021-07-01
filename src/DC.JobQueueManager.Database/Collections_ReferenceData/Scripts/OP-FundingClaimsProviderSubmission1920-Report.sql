
DECLARE @CollectionId_OP_FundingClaimsProviderSubmission1920_Report INT = 176

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_OP_FundingClaimsProviderSubmission1920_Report AS CollectionId,N'OP-FundingClaimsProviderSubmission1920-Report' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, 1920 AS CollectionYear, N'Funding Claims Provider Submissions 1920 Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_FundingClaimsProviderSubmission1920_Report As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateFundingClaimsProviderSubmissionsReport1920' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------


