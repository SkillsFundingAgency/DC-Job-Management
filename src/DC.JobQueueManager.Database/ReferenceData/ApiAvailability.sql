BEGIN

	DECLARE @SummaryOfChanges_ApiAvailability TABLE ([ApiName] NVARCHAR(200),[Process] NVARCHAR(200), [Action] VARCHAR(100));

	MERGE INTO [dbo].[ApiAvailability] AS Target
	USING (VALUES
			(N'Learner', N'PE-ILR')		
		  )
		AS Source([ApiName],[Process])
			ON Target.[ApiName] = Source.[ApiName] AND Target.[Process] = Source.[Process]
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.[ApiName]
								  ,Target.[Process]
						EXCEPT 
							SELECT Source.[ApiName]
								  ,Source.[Process]
					)
			  THEN UPDATE SET Target.[ApiName] = Source.[ApiName],
							  Target.[Process] = Source.[Process]
		WHEN NOT MATCHED BY TARGET THEN INSERT([ApiName],[Process]) 
									   VALUES ([ApiName],[Process])
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.[ApiName], Inserted.[Process],$action INTO @SummaryOfChanges_ApiAvailability([ApiName],[Process],[Action])
	;

		DECLARE @AddCount_AA INT, @UpdateCount_AA INT, @DeleteCount_AA INT
		SET @AddCount_AA  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ApiAvailability WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_AA = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ApiAvailability WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_AA = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ApiAvailability WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'     ApiAvailability', @AddCount_AA, @UpdateCount_AA, @DeleteCount_AA) WITH NOWAIT;
END
GO