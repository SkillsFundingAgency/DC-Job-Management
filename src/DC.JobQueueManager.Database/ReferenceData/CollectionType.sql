
BEGIN

	DECLARE @SummaryOfChanges_CollectionType TABLE ([CollectionTypeId] INT, [Action] VARCHAR(100));

	MERGE INTO [dbo].[CollectionType] AS Target
	USING (VALUES
			(1, N'ILR', N'Individualised Learner Record', 1, 1),
			(2, N'EAS', N'Earnings Adjustment Statement', 1, 1),
			(3, N'ESF', N'European Social Fund supplementary data', 1, 1),
			(4, N'REF', N'Internal reference data job', 0,0),
			(5, N'NCS', N'Upload NCS (National Careers Service) data', 1,1),
			(6, N'FC', N'Funding Claims', 0,1),
			(7, N'PE', N'Period End', 0,0),
			(8, N'Publication', N'Reports Publication', 0,1),
			(9, N'OP', N'Operations', 0,0),
			(10, N'Generic', N'Generic collection', 1,0),
			(11, N'COVID', N'Provider Relief Scheme', 1,0),
			(12, N'COVID2', N'Provider Relief Scheme 2', 1,0),
			(13, N'PE-ILR', N'Period End ILR', 0,0),
			(14, N'PE-NCS', N'Period End NCS', 0,0),
			(15, N'PE-ALLF', N'Period End ALLF', 0,0),
			(16, N'MCA-GLA', N'Devolved Contracts', 0,1),
			(17, N'ESFR', N'European Social Fund Reprofiling', 1,1),
			(18, N'FEW', N'FE Workforce', 1,1),
			(19, N'MCA-GLA-FA', N'MCA GLA Funded Aims', 0,1)

		  )
		AS Source([CollectionTypeId], [Type], [Description], [IsManageableInOperations], [IsProviderAssignableInOperations])
			ON Target.[CollectionTypeId] = Source.[CollectionTypeId]
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.[Description]
								  ,Target.[Type]
								  ,Target.[IsManageableInOperations]
								  ,Target.[IsProviderAssignableInOperations]
						EXCEPT 
							SELECT Source.[Description]
								  ,Source.[Type]
								  ,Source.[IsManageableInOperations]
								  ,Source.[IsProviderAssignableInOperations]
					)
			  THEN UPDATE SET Target.[Description] = Source.[Description],
							  Target.[Type] = Source.[Type],
							  Target.[IsManageableInOperations] = Source.[IsManageableInOperations],
							  Target.[IsProviderAssignableInOperations] = Source.[IsProviderAssignableInOperations]
		WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionTypeId], [Type], [Description], [IsManageableInOperations], [IsProviderAssignableInOperations]) 
									   VALUES ([CollectionTypeId], [Type], [Description], [IsManageableInOperations], [IsProviderAssignableInOperations])
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.[CollectionTypeId],$action INTO @SummaryOfChanges_CollectionType([CollectionTypeId],[Action])
	;

		DECLARE @AddCount_CT INT, @UpdateCount_CT INT, @DeleteCount_CT INT
		SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionType WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionType WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionType WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'     CollectionType', @AddCount_CT, @UpdateCount_CT, @DeleteCount_CT) WITH NOWAIT;
END
GO
