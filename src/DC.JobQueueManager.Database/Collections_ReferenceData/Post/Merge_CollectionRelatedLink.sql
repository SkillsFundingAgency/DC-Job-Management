
	DECLARE @SummaryOfChanges_RelatedLinks TABLE ([CollectionId] INT, [Title] nVARCHAR(MAX), [Url] nVARCHAR(max), [SortOrder] int, [TriggerPointName] VARCHAR(250), [Action] VARCHAR(100));

	MERGE INTO [CollectionRelatedLink] AS Target
	USING (
			SELECT [CollectionId], [Title], [Url], [SortOrder]
			FROM @TempCollectionRelatedLink
		  )
		AS Source([CollectionId], [Title], [Url], [SortOrder])
			ON Target.CollectionId = Source.CollectionId
			AND Target.Title = Source.Title
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.[CollectionId]
								  ,Target.[Title]
								  ,Target.[Url]
								  ,Target.[SortOrder]
						EXCEPT 
							SELECT Source.[CollectionId]
								  ,Source.[Title]
								  ,Source.[Url]
								  ,Source.[SortOrder]
					)
			  THEN UPDATE SET Target.[CollectionId] = Source.[CollectionId],
							  Target.[Title] = Source.[Title],
							  Target.[Url] = Source.[Url],
							  Target.[SortOrder] = Source.[SortOrder]
		WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionId], [Title], [Url], [SortOrder]) 
									   VALUES ([CollectionId], [Title], [Url], [SortOrder])
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.CollectionId, Inserted.[Title], Inserted.[Url], Inserted.[SortOrder], 
		$action INTO @SummaryOfChanges_RelatedLinks([CollectionId], [Title], [Url], [SortOrder], [Action])
	;

		DECLARE @AddCount_CT INT, @UpdateCount_CT INT, @DeleteCount_CT INT
		SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_RelatedLinks WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_RelatedLinks WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_RelatedLinks WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'     Li', @AddCount_CT, @UpdateCount_CT, @DeleteCount_CT) WITH NOWAIT;
