
	DECLARE @SummaryOfChanges_JobTopic TABLE ([DeletedTopicName] varchar(max),[TopicName] varchar(max), [Action] VARCHAR(100));
	DECLARE @SummaryOfChanges_JobTopicTasks TABLE ([DeletedTaskName] varchar(max), [TaskName] varchar(max), [Action] VARCHAR(100));

	SELECT * from @TempTopicTasks
	
	MERGE INTO [dbo].[JobTopicSubscription] AS Target
	USING (
			Select Distinct [CollectionId],[SubscriptionName],[TopicName],[TopicOrder],[IsFirstStage],TopicEnabled from @TempTopicTasks
		) 
		AS Source([CollectionId],[SubscriptionName],[TopicName],[TopicOrder],[IsFirstStage],[Enabled])
		
		ON Target.[CollectionId] = Source.[CollectionId]
			And IsNull(Target.IsFirstStage,0) = IsNull(Source.IsFirstStage,0)
			And Target.TopicOrder =  Source.TopicOrder
		
		WHEN MATCHED
				AND EXISTS
					(		SELECT Target.[CollectionId]
								  ,Target.[SubscriptionName]
								  ,Target.[TopicName]
								  ,Target.[TopicOrder]
								  ,Target.[IsFirstStage]
								  ,Target.[Enabled]
						EXCEPT
							SELECT Source.[CollectionId]
								  ,Source.[SubscriptionName]
								  ,Source.[TopicName]
								  ,Source.[TopicOrder]
								  ,Source.[IsFirstStage]
								  ,Source.[Enabled]
					)
			  THEN UPDATE SET Target.[CollectionId] = Source.[CollectionId],
							  Target.[SubscriptionName] = Source.[SubscriptionName],
							  Target.[TopicName] = Source.[TopicName],
							  Target.[TopicOrder] = Source.[TopicOrder],
							  Target.[IsFirstStage] = Source.[IsFirstStage],
							  Target.[Enabled] = Source.[Enabled]

		WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionId],[SubscriptionName],[TopicName],[TopicOrder],[IsFirstStage],[Enabled] )
									   VALUES ([CollectionId],[SubscriptionName],[TopicName],[TopicOrder],[IsFirstStage],[Enabled] )
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Deleted.TopicName,Inserted.TopicName,$action INTO @SummaryOfChanges_JobTopic(DeletedTopicName,[TopicName],[Action]);

		

		-- Tasks update
		MERGE INTO [dbo].[JobSubscriptionTask] AS Target
	USING (
			Select Distinct jst.JobTopicId, tmp.[CollectionId],tmp.[SubscriptionName],tmp.[TopicName],tmp.[IsFirstStage], TaskName, TaskOrder from @TempTopicTasks tmp
			Inner Join JobTopicSubscription jst on 
			tmp.[CollectionId] = jst.[CollectionId]
			And tmp.[SubscriptionName] = jst.[SubscriptionName]
			And tmp.TopicName = jst.TopicName
			And tmp.TopicOrder = jst.TopicOrder
			And IsNull(tmp.IsFirstStage,0) = IsNull(jst.IsFirstStage,0)

		) 
		AS Source(JobTopicId,[CollectionId],[SubscriptionName],[TopicName],[IsFirstStage], TaskName, TaskOrder)
		
		ON Target.JobTopicId = Source.JobTopicId
			And Target.TaskName = Source.TaskName
			And Target.TaskOrder =  Source.TaskOrder

	WHEN MATCHED
				AND EXISTS
					(		SELECT Target.[JobTopicId]
								  ,Target.TaskName
								  ,Target.[TaskOrder]
						EXCEPT
							SELECT Source.[JobTopicId]
								  ,Source.[TaskName]
								  ,Source.[TaskOrder]
					)
			  THEN UPDATE SET Target.[JobTopicId] = Source.[JobTopicId],
							  Target.[TaskName] = Source.[TaskName],
							  Target.[TaskOrder] = Source.[TaskOrder]

		WHEN NOT MATCHED BY TARGET THEN INSERT([JobTopicId],[TaskName],[TaskOrder],[Enabled])
									   VALUES ([JobTopicId],[TaskName],[TaskOrder],0)
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Deleted.TaskName,Inserted.TaskName,$action INTO @SummaryOfChanges_JobTopicTasks([DeletedTaskName], TaskName,[Action]);

----

		DECLARE @AddCount_JBT INT, @UpdateCount_JBT INT, @DeleteCount_JBT INT
		SET @AddCount_JBT  = ISNULL((SELECT COUNT(*) FROM @SummaryOfChanges_JobTopic WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_JBT = ISNULL((SELECT COUNT(*) FROM @SummaryOfChanges_JobTopic WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_JBT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobTopic WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		       %s - Added %i - Update %i - Delete %i',10,1,'JobTopicSubscription', @AddCount_JBT, @UpdateCount_JBT, @DeleteCount_JBT) WITH NOWAIT;

--------------------------------------------------------------------
		-- Display all topic changes 
		DECLARE @DeletedTopicName VARCHAR(MAX), @TopicName VARCHAR(MAX), @Action   varchar(100);
		DECLARE topics_change_cursor CURSOR
			FOR SELECT DeletedTopicName,[TopicName],[Action] FROM @SummaryOfChanges_JobTopic;
			
			OPEN topics_change_cursor;

			FETCH NEXT FROM topics_change_cursor INTO 
				@DeletedTopicName,
				@TopicName, 
				@Action;

			WHILE @@FETCH_STATUS = 0
				BEGIN
				if (@TopicName is null)
					RAISERROR('		       %s - %s - %s',10,1,'JobTopicSubscription', @DeletedTopicName, @Action) WITH NOWAIT;
				Else
					RAISERROR('		       %s - %s - %s',10,1,'JobTopicSubscription',  @TopicName, @Action) WITH NOWAIT;
				FETCH NEXT FROM topics_change_cursor INTO 
					@DeletedTopicName,
					@TopicName, 
					@Action;
					
			END;
			CLOSE topics_change_cursor;
			DEALLOCATE topics_change_cursor;
--------------------------------------------------------------------

		DECLARE @AddCount_JTT INT, @UpdateCount_JTT INT, @DeleteCount_JTT INT
		SET @AddCount_JTT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobTopicTasks WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_JTT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobTopicTasks WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_JTT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobTopicTasks WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'JobSubscriptionTask', @AddCount_JTT, @UpdateCount_JTT, @DeleteCount_JTT) WITH NOWAIT;

		-- Display all topic changes 
		DECLARE @DeletedTaskName VARCHAR(MAX), @TaskName VARCHAR(MAX), @TaskAction   varchar(100);
		DECLARE tasks_change_cursor CURSOR
			FOR SELECT DeletedTaskName,TaskName,[Action] FROM @SummaryOfChanges_JobTopicTasks;
			
			OPEN tasks_change_cursor;

			FETCH NEXT FROM tasks_change_cursor INTO 
				@DeletedTaskName,
				@TaskName, 
				@TaskAction  ;

			WHILE @@FETCH_STATUS = 0
				BEGIN
				if (@TaskName is null)
					RAISERROR('		       %s - %s - %s',10,1,'JobSubscriptionTask', @DeletedTaskName, @TaskAction  ) WITH NOWAIT;
				Else
					RAISERROR('		       %s - %s - %s',10,1,'JobSubscriptionTask',  @TaskName, @TaskAction  ) WITH NOWAIT;
				FETCH NEXT FROM tasks_change_cursor INTO 
				@DeletedTaskName,
				@TaskName, 
				@TaskAction  ;
					
			END;
			CLOSE tasks_change_cursor;
			DEALLOCATE tasks_change_cursor;