
DECLARE @CollectionId_PE_DAS_AppsCoInvestmentContributionsReport1920 INT = 65

--ADD collection
INSERT INTO @TempCollection([CollectionId],[Name],[IsOpen],[CollectionType],[CollectionYear],[Description],[SubText],[CrossloadingEnabled],[ProcessingOverrideFlag],[MultiStageProcessing],[StorageReference],[FileNameRegex],[ResubmitJob])
SELECT @CollectionId_PE_DAS_AppsCoInvestmentContributionsReport1920 AS CollectionId,N'PE-DAS-AppsCoInvestmentContributionsReport1920' AS Name, 1 AS IsOpen, N'PE-ILR' AS CollectionType, 1920 AS CollectionYear, null AS Description, null AS SubText, null AS CrossloadingEnabled, null As ProcessingOverrideFlag, 0 AS MultiStageProcessing, N'periodend1920-files' AS StorageReference,null AS FileNameRegex, 1 AS ResubmitJob


----------------------------------------------------------------------------------------------------------------------------------------


--ADD Topics/Tasks

INSERT INTO @TempTopicTasks
 SELECT @CollectionId_PE_DAS_AppsCoInvestmentContributionsReport1920 As CollectionId,N'Reports1920' As SubscriptionName,N'periodendtopic' As TopicName,1 As TopicOrder,0 As IsFirstStage,1 As TopicEnabled,N'TaskGenerateAppsCoInvestmentContributionsReport' As TaskName,1 As TaskOrder

----------------------------------------------------------------------------------------------------------------------------------------

