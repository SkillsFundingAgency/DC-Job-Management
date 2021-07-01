CREATE VIEW [dbo].[vw_DisplayJobTaskList]
AS	
		
	SELECT TOP 100 percent
		    --JG.[Description] as JobGroup
		   --,JG.[ConcurrentExecutionCount] as [JobGroupConcurrentExecutionCount]
		   J.Name as Collection
		   ,JT.[TopicName] as Topic
		   ,JT.[SubscriptionName] as SubscriptionName
		   ,JT.[Enabled] as TopicEnabled
		   ,JTT.[TaskName] as TaskName
		   ,JTT.[Enabled] as TaskEnabled
		   ,ISNULL(JT.[IsFirstStage],0) as [IsFirstStage]
		   ,J.[MultiStageProcessing] as [IsCrossLoadingEnabled]
		   ,[ProcessingOverrideFlag]
		  --,JG.*
		  --,J.*
		  --,JT.*
		  --,JTT.*
	FROM  [dbo].[Collection] J
	INNER JOIN [dbo].[JobTopicSubscription] JT 
		ON JT.CollectionId = J.CollectionId
	LEFT JOIN [dbo].[JobSubscriptionTask] JTT
		ON JTT.[JobTopicId] = JT.[JobTopicId]	
	
	Where jt.Enabled = 1 and IsNull(jtt.Enabled,1) = 1

	ORDER BY 
		 --ISNULL(JG.[JobTypeGroupId],0) ASC
		J.[CollectionId] ASC
		,ISNULL(JT.[IsFirstStage],0) DESC
		,JT.[TopicOrder] ASC
		,JTT.[TaskOrder] ASC
  