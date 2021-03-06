
DECLARE @CollectionId_OP_2021_MidYear_FundingClaimsSavedButNotSubmitted_Report INT = 2021135

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_OP_2021_MidYear_FundingClaimsSavedButNotSubmitted_Report AS CollectionId,N'2021-MidYear-FundingClaimsSavedButNotSubmittedReport' AS Name, 1 AS IsOpen, N'OP' AS CollectionType, 2021 AS CollectionYear, N'2021 Mid Year Funding Claims Saved But Not Submitted Report' AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'opsreferencedata' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


--------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_OP_2021_MidYear_FundingClaimsSavedButNotSubmitted_Report As CollectionId,N'Reports' As SubscriptionName,N'operationstopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerate2021MidYearFundingClaimsSavedButNotSubmittedReport' As TaskName,1 As TaskOrder

--------------------------------------------------------------------------------------------------------------------------------------

