DECLARE @JobTopicSubscription_TobeDeleted TABLE (JobTopicId INT, CollectionId INT, TopicName nvarchar(100), SubscriptionName nvarchar(100));

DECLARE @JobSubscriptionTask_TobeDeleted TABLE (JobTopicTaskId INT, JobTopicId INT, TaskName NVARCHAR(500));

DECLARE @Collection_ToBeDeleted  TABLE (CollectionId INT);


INSERT INTO @JobTopicSubscription_TobeDeleted
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202160 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202161 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202162 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202164 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202165 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202180 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202184 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202185 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202186 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202193 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202194 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202199 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 202182 AND TopicName = 'periodendtopic' AND SubscriptionName = 'MCAGLAReporting'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 2021130 AND TopicName = 'periodendtopic' AND SubscriptionName = 'MCAGLAReporting'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 60 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 61 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 62 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 64 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 65 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 80 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 84 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 85 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 86 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 93 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 94 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 99 AND TopicName = 'periodendtopic' AND SubscriptionName = 'Reports'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 82 AND TopicName = 'periodendtopic' AND SubscriptionName = 'MCAGLAReporting'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 130 AND TopicName = 'periodendtopic' AND SubscriptionName = 'MCAGLAReporting'
UNION
SELECT JobTopicId, CollectionId, TopicName, SubscriptionName FROM [JobTopicSubscription] WHERE CollectionId= 166 AND TopicName = 'referencedatatopic' AND SubscriptionName = 'Process'

INSERT INTO @JobSubscriptionTask_TobeDeleted
SELECT [JobTopicTaskId], [JobTopicId], [TaskName] FROM [dbo].[JobSubscriptionTask] WHERE JobTopicId IN (SELECT JobTopicId FROM @JobTopicSubscription_TobeDeleted)
UNION
SELECT [JobTopicTaskId], [JobTopicId], [TaskName]  FROM [JobSubscriptionTask] WHERE TaskName in ('Apps2021_EAS', 'Apps2021_Levy', 'Apps2021_NonLevy')
UNION
SELECT [JobTopicTaskId], [JobTopicId], [TaskName]  FROM [JobSubscriptionTask] WHERE TaskName = 'FundingClaimsProviderData'

INSERT INTO @Collection_ToBeDeleted
SELECT CollectionId FROM [dbo].[Collection] WHERE Name = 'FundingClaimsProviderData'

RAISERROR('		       JobSubscriptionTasks to be deleted',10,1) WITH NOWAIT;

SELECT * FROM @JobSubscriptionTask_TobeDeleted 

DELETE FROM [dbo].[JobSubscriptionTask] WHERE JobTopicTaskId IN (SELECT JobTopicTaskId FROM @JobSubscriptionTask_TobeDeleted)

RAISERROR('		       JobTopicSubscriptions to be deleted',10,1) WITH NOWAIT;

SELECT * FROM @JobTopicSubscription_TobeDeleted

DELETE FROM JobTopicSubscription WHERE JobTopicId IN (SELECT JobTopicId FROM @JobTopicSubscription_TobeDeleted)

RAISERROR('		       Collections to be deleted',10,1) WITH NOWAIT;

SELECT * FROM @Collection_ToBeDeleted

DELETE FROM Collection WHERE CollectionId IN (SELECT CollectionId FROM @Collection_ToBeDeleted)