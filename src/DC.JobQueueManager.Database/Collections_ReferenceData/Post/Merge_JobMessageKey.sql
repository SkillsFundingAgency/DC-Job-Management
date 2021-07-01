

DECLARE @SummaryOfChanges_JobMessageKey TABLE ([MessageKey] varchar(100), [Action] VARCHAR(100));

MERGE INTO [dbo].[JobMessageKey] AS Target
USING (	
			SELECT NewRecords.CollectionId, NewRecords.[MessageKey], NewRecords.[IsFirstStage] 
			FROM 
			(
				SELECT CollectionId , MessageKey,  IsFirstStage 
				FROM @TempJobMessageKey

		 ) NewRecords
	  )
	AS Source(CollectionId, MessageKey, IsFirstStage)
		ON IsNull(Target.CollectionId,0) = IsNull(Source.CollectionId,0)
		And Target.MessageKey = Source.MessageKey
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.CollectionId
							  ,Target.MessageKey
							  ,Target.IsFirstStage
					EXCEPT 
						SELECT Source.CollectionId
						      ,Source.MessageKey
							  ,Source.IsFirstStage
				)
		  THEN UPDATE SET Target.CollectionId = Source.CollectionId,
			              Target.MessageKey = Source.MessageKey,
						  Target.IsFirstStage = Source.IsFirstStage

	WHEN NOT MATCHED BY TARGET THEN INSERT(CollectionId, MessageKey, IsFirstStage) 
								   VALUES (CollectionId, MessageKey, IsFirstStage)
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT  ISNULL(deleted.MessageKey,Inserted.MessageKey),$action INTO @SummaryOfChanges_JobMessageKey([MessageKey],[Action])
;

	DECLARE @AddCount_JMK INT, @UpdateCount_JMK INT, @DeleteCount_JMK INT
	SET @AddCount_JMK  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobMessageKey WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_JMK = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobMessageKey WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_JMK = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobMessageKey WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		         %s - Added %i - Update %i - Delete %i',10,1,'     JobMessageKey', @AddCount_JMK, @UpdateCount_JMK, @DeleteCount_JMK) WITH NOWAIT;

