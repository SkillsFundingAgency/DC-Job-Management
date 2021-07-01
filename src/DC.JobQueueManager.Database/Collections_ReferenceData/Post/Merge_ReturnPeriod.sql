
SET NOCOUNT ON;

	DECLARE @SummaryOfChanges_ReturnPeriod TABLE ([CollectionId] INT, [PeriodNumber] INT, [Action] VARCHAR(100));
	DECLARE @SummaryOfChanges_DateTimeUpdates TABLE ([CollectionId] INT, [PeriodNumber] INT);
	
	BEGIN TRAN

	BEGIN TRY

		INSERT INTO @SummaryOfChanges_DateTimeUpdates 
		SELECT  [Source].[CollectionId], [Source].[PeriodNumber]
		FROM @TempReturnPeriod Source
		JOIN dbo.ReturnPeriod Target
			ON source.[CollectionId] = Target.[CollectionId]
			AND source.[PeriodNumber] = Target.[PeriodNumber]
		WHERE (Source.[StartDateTimeUTC] <> Target.[StartDateTimeUTC] OR Source.[EndDateTimeUTC] <> Target.[EndDateTimeUTC]);
	

		MERGE INTO [dbo].[ReturnPeriod] AS Target
		USING (
				SELECT  [CollectionId], [PeriodNumber], [StartDateTimeUTC], [EndDateTimeUTC],  MONTH([StartDateTimeUTC]) As [CalendarMonth], YEAR([StartDateTimeUTC]) AS [CalendarYear]
				FROM @TempReturnPeriod
				)
			AS Source([CollectionId], [PeriodNumber], [StartDateTimeUTC], [EndDateTimeUTC], [CalendarMonth], [CalendarYear])
				ON Target.[CollectionId] = Source.[CollectionId]
			AND Target.[PeriodNumber] = Source.[PeriodNumber]

			WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionId],[StartDateTimeUTC], [EndDateTimeUTC], [PeriodNumber], [CalendarMonth], [CalendarYear]) 
											VALUES ([CollectionId],[StartDateTimeUTC], [EndDateTimeUTC], [PeriodNumber], [CalendarMonth], [CalendarYear])
			WHEN NOT MATCHED BY Source THEN DELETE
			OUTPUT ISNULL(Inserted.[CollectionId],deleted.[CollectionId]),
					ISNULL(Inserted.[PeriodNumber],deleted.[PeriodNumber]),
					$action 
				INTO @SummaryOfChanges_ReturnPeriod([CollectionId],[PeriodNumber],[Action])
			;

			DECLARE @AddCount_RT_ReturnPeriods INT, @UpdateCount_RT_ReturnPeriods INT, @DeleteCount_RT_ReturnPeriods INT
			SET @AddCount_RT_ReturnPeriods  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ReturnPeriod WHERE [Action] = 'Insert' GROUP BY Action),0);
			SET @UpdateCount_RT_ReturnPeriods = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ReturnPeriod WHERE [Action] = 'Update' GROUP BY Action),0);
			SET @DeleteCount_RT_ReturnPeriods = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ReturnPeriod WHERE [Action] = 'Delete' GROUP BY Action),0);

			RAISERROR('		      %s : %s - Added %i - Update %i - Delete %i',10,1,'        ReturnPeriod', 'Collection', @AddCount_RT_ReturnPeriods, @UpdateCount_RT_ReturnPeriods, @DeleteCount_RT_ReturnPeriods) WITH NOWAIT;

			SELECT t.*, soc.Action FROM @TempreturnPeriod t LEFT JOIN @SummaryOfChanges_ReturnPeriod soc ON t.[CollectionId] = soc.[CollectionId] AND t.[PeriodNumber] = soc.[PeriodNumber]
			
			COMMIT

			IF EXISTS(SELECT TOP 1 * FROM @SummaryOfChanges_DateTimeUpdates)
			BEGIN

				DECLARE 
					@collectionId INT, 
					@periodNumber INT;

				DECLARE cursor_updates CURSOR
				FOR SELECT 
						CollectionId, 
						PeriodNumber	
					FROM 
						@SummaryOfChanges_DateTimeUpdates;
				
				OPEN cursor_updates;

				FETCH NEXT FROM cursor_updates INTO 
					@collectionId, 
					@periodNumber;

				WHILE @@FETCH_STATUS = 0
					BEGIN
						PRINT 'Return Period Update detected and ignored for CollectionId : ' + Convert(Varchar, @collectionId) + ' , Period number : '+ CAST(@periodNumber AS varchar);
						FETCH NEXT FROM cursor_updates INTO 
							@collectionId, 
							@periodNumber;
					END;

				CLOSE cursor_updates;  
				DEALLOCATE cursor_updates;  
			END


	END TRY
 
---------------------------------------------------------------------------------- 
-- Handle any problems
---------------------------------------------------------------------------------- 
 
BEGIN CATCH

	DECLARE   @ErrorMessage_ReturnPeriods		NVARCHAR(4000)
			, @ErrorSeverity_ReturnPeriods	INT 
			, @ErrorState_ReturnPeriods		INT
			, @ErrorNumber_ReturnPeriods		INT
						
	SELECT	  @ErrorNumber_ReturnPeriods		= ERROR_NUMBER()
			, @ErrorMessage_ReturnPeriods		= 'Error in :' + ISNULL(OBJECT_NAME(@@PROCID),'') + ' - Error was :' + ERROR_MESSAGE()
			, @ErrorSeverity_ReturnPeriods	= ERROR_SEVERITY()
			, @ErrorState_ReturnPeriods		= ERROR_STATE();
	
	IF (@@TRANCOUNT>0)
		ROLLBACK;
		
	RAISERROR (  @ErrorMessage_ReturnPeriods		-- Message text.
				, @ErrorSeverity_ReturnPeriods	-- Severity.
				, @ErrorState_ReturnPeriods		-- State.
				);
				  
END CATCH
 
---------------------------------------------------------------------------------- 
-- All done
---------------------------------------------------------------------------------- 
