
DECLARE @CollectionId_OP_2021_YearEnd_FundingClaimsProviderSubmission_Report INT = 2021141

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob], [EmailOnJobCreation])
SELECT @CollectionId_OP_2021_YearEnd_FundingClaimsProviderSubmission_Report AS CollectionId,N'2021-YearEnd-FundingClaimsProviderSubmissionReport' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, 2021 AS CollectionYear, N'2021 Year-End Funding Claims Provider Submissions Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, 'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob, 0 As EmailOnJobCreation

----------------------------------------------------------------------------------------------------------------------------------------

--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_2021_YearEnd_FundingClaimsProviderSubmission_Report As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerate2021YearEndFundingClaimsProviderSubmissionReport' As TaskName,1 As TaskOrder


----------------------------------------------------------------------------------------------------------------------------------------


