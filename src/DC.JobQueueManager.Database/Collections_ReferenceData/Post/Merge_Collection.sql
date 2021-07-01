
	DECLARE @SummaryOfChanges_Collection TABLE ([CollectionId] INT, [Action] VARCHAR(100));

	MERGE INTO [dbo].[Collection] AS Target
	USING (
			SELECT 
				NewRecords.[CollectionId],
				NewRecords.[Name],
				NewRecords.[IsOpen],
				CT.[CollectionTypeId],
				NewRecords.[CollectionYear],
				NewRecords.[Description],
				NewRecords.[SubText],
				NewRecords.CrossloadingEnabled,
				NewRecords.ProcessingOverrideFlag,
				NewRecords.MultiStageProcessing,
				NewRecords.StorageReference,
				NewRecords.FileNameRegex,
				NewRecords.ResubmitJob,
				NewRecords.EmailOnJobCreation 
			FROM
			(
					 SELECT
						[CollectionId],
						[Name],
						[IsOpen],
						[CollectionType],
						[CollectionYear],
						[Description],
						[SubText],
						CrossloadingEnabled,
						ProcessingOverrideFlag,
						MultiStageProcessing,
						StorageReference,
						FileNameRegex,
						ResubmitJob,
						EmailOnJobCreation 
					 FROM @TempCollection

			) AS NewRecords
			INNER JOIN [dbo].[CollectionType] CT
				ON CT.[TYPE] = NewRecords.[CollectionType]
		  )
		AS Source(
			[CollectionId],
			[Name],
			[IsOpen],
			[CollectionTypeId],
			[CollectionYear],
			[Description],
			[SubText],
			CrossloadingEnabled,
			ProcessingOverrideFlag,
			MultiStageProcessing,
			StorageReference,
			FileNameRegex,
			ResubmitJob,
			EmailOnJobCreation)
			ON Target.[CollectionId] = Source.[CollectionId]
		WHEN MATCHED
				AND EXISTS
					(		SELECT Target.[Name]
								  ,Target.[IsOpen]
								  ,Target.[CollectionTypeId]
								  ,Target.[CollectionYear]
								  ,Target.[Description]
								  ,Target.[SubText]
								  ,Target.CrossloadingEnabled
								  ,Target.ProcessingOverrideFlag
								  ,Target.MultiStageProcessing
								  ,Target.StorageReference COLLATE Latin1_General_100_CS_AS
								  ,Target.FileNameRegex
								  ,Target.ResubmitJob
								  ,Target.EmailOnJobCreation
						EXCEPT
							SELECT Source.[Name]
								  ,Source.[IsOpen]
								  ,Source.[CollectionTypeId]
								  ,Source.[CollectionYear]
								  ,Source.[Description]
								  ,Source.[SubText]
								  ,Source.CrossloadingEnabled
								  ,Source.ProcessingOverrideFlag
								  ,Source.MultiStageProcessing
								  ,Source.StorageReference COLLATE Latin1_General_100_CS_AS
								  ,Source.FileNameRegex
								  ,Source.ResubmitJob
								  ,Source.EmailOnJobCreation
					)
			  THEN UPDATE SET	Target.[Name] = Source.[Name],
								Target.[IsOpen] = Source.[IsOpen],
								Target.[CollectionTypeId] = Source.[CollectionTypeId],
								Target.[CollectionYear] = Source.[CollectionYear],
								Target.[Description] = Source.[Description],
								Target.[SubText] = Source.[SubText],
								Target.CrossloadingEnabled = source.CrossloadingEnabled,
								Target.ProcessingOverrideFlag = source.ProcessingOverrideFlag,
								Target.MultiStageProcessing = source.MultiStageProcessing,
								Target.StorageReference = source.StorageReference,
								Target.FileNameRegex = source.FileNameRegex,
								Target.ResubmitJob = Source.ResubmitJob,
								Target.EmailOnJobCreation = Source.EmailOnJobCreation


		WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionId], [IsOpen], [Name], [CollectionTypeId], [CollectionYear], [Description], [SubText], CrossloadingEnabled,ProcessingOverrideFlag,MultiStageProcessing,StorageReference,FileNameRegex,ResubmitJob,EmailOnJobCreation)
									   VALUES ([CollectionId], [IsOpen], [Name], [CollectionTypeId], [CollectionYear], [Description], [SubText], CrossloadingEnabled,ProcessingOverrideFlag,MultiStageProcessing,StorageReference,FileNameRegex,ResubmitJob,EmailOnJobCreation)
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.[CollectionId],$action INTO @SummaryOfChanges_Collection([CollectionId],[Action]);

		DECLARE @AddCount_C INT, @UpdateCount_C INT, @DeleteCount_C INT
		SET @AddCount_C  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Collection WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Collection WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Collection WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		         %s - Added %i - Update %i - Delete %i',10,1,'        Collection', @AddCount_C, @UpdateCount_C, @DeleteCount_C) WITH NOWAIT;
