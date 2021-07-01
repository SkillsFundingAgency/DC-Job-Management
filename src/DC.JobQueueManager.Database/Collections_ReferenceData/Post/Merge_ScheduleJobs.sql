
SET NOCOUNT ON;
--BEGIN TRAN;

DECLARE @SummaryOfChanges_Schedule TABLE ([Id] INT, [Action] VARCHAR(100));
DECLARE @TimeOffSet INT = 0;

SELECT @TimeOffSet = CASE PARSENAME(REPLACE(@@SERVERNAME,'-','.'),3)
						WHEN 'pp'    THEN 2
						WHEN 'pd'    THEN 4
						WHEN 'dev'   THEN 6
						WHEN 'devci' THEN 8
						WHEN 'tst'   THEN 10
						WHEN 'sit'   THEN 12
						WHEN 'ops'   THEN 14
						WHEN 'das'   THEN 16
						WHEN 'dsi'   THEN 18
						WHEN 'dst'   THEN 20
						WHEN 'dsp'   THEN 22
						WHEN 'e2e'   THEN 24
						ELSE 40
					 END 

SET IDENTITY_INSERT [dbo].[Schedule] ON;	

MERGE INTO [dbo].[Schedule] AS Target
USING (
			SELECT --TOP 100 PERCENT
			       [Id]
				  ,JT.[CollectionId]
				  ,[JobTitle]
				  ,[MinuteIsCadence]
				  ,CASE WHEN [JobTitle] LIKE '%FCS' THEN 
							CASE WHEN ([Minute] + ISNULL(@TimeOffSet,1)) > 59 
								THEN (([Minute] + ISNULL(@TimeOffSet,1)) - 60) 
								ELSE ([Minute] + ISNULL(@TimeOffSet,1)) END
	 					ELSE [Minute]
				   END as [Minute]
				  ,CASE WHEN [JobTitle] LIKE '%FCS' THEN 
						CASE WHEN ([Minute] + ISNULL(@TimeOffSet,1)) > 59 THEN IsNull([Hour],0) + 1 ELSE [HOUR] END
					ELSE [Hour] END As [Hour]
				  ,NULL AS [DayOfTheMonth]
				  ,NULL AS [Month]
				  ,NULL AS [DayOfTheWeek]
				  ,0 AS [ExecuteOnceOnly]
			FROM @TempSchedule as Dat
			INNER JOIN [dbo].Collection JT 
				ON JT.[Name] = Dat.[JobTitle]
	)
	AS Source( [Id]
              ,[CollectionId]
              ,[JobTitle]
              ,[MinuteIsCadence]
              ,[Minute]
              ,[Hour]
              ,[DayOfTheMonth]
              ,[Month]
              ,[DayOfTheWeek]
              ,[ExecuteOnceOnly]
			 )
	ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[CollectionId]
							  ,Target.[MinuteIsCadence]
							  ,Target.[Minute]
							  ,Target.[Hour]
							  ,Target.[DayOfTheMonth]
							  ,Target.[Month]
							  ,Target.[DayOfTheWeek]
							  ,Target.[ExecuteOnceOnly]				  
					EXCEPT 
						SELECT Source.[CollectionId]
							  ,Source.[MinuteIsCadence]
							  ,Source.[Minute]
							  ,Source.[Hour]
							  ,Source.[DayOfTheMonth]
							  ,Source.[Month]
							  ,Source.[DayOfTheWeek]
							  ,Source.[ExecuteOnceOnly]
				)
		  THEN UPDATE SET Target.[MinuteIsCadence] = Source.[MinuteIsCadence]
			             ,Target.[Minute] = Source.[Minute]
			             ,Target.[Hour] = Source.[Hour]
			             ,Target.[DayOfTheMonth] = Source.[DayOfTheMonth]
			             ,Target.[Month] = Source.[Month]
			             ,Target.[DayOfTheWeek] = Source.[DayOfTheWeek]
			             ,Target.[CollectionId] = Source.[CollectionId]
			             ,Target.[ExecuteOnceOnly] = Source.[ExecuteOnceOnly]
	WHEN NOT MATCHED BY TARGET THEN INSERT([Id],[Enabled],[MinuteIsCadence],[Minute],[Hour],[DayOfTheMonth],[Month],[DayOfTheWeek],[CollectionId],[ExecuteOnceOnly]) 
								    VALUES (Source.[Id]
										   ,0
										   ,Source.[MinuteIsCadence]
										   ,Source.[Minute]
										   ,Source.[Hour]
										   ,Source.[DayOfTheMonth]
										   ,Source.[Month]
										   ,Source.[DayOfTheWeek]
										   ,Source.[CollectionId]
										   ,Source.[ExecuteOnceOnly]
										   )
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT ISNULL(Deleted.[Id],Inserted.[Id]),$action INTO @SummaryOfChanges_Schedule([Id],[Action])
	;

	DECLARE @AddCount_ShedJob INT, @UpdateCount_ShedJob INT, @DeleteCount_ShedJob INT
	SET @AddCount_ShedJob    = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Schedule WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_ShedJob = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Schedule WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_ShedJob = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Schedule WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		               %s - Added %i - Update %i - Delete %i',10,1,'Schedule', @AddCount_ShedJob, @UpdateCount_ShedJob, @DeleteCount_ShedJob) WITH NOWAIT;

	SET IDENTITY_INSERT [dbo].[Schedule] OFF;	

	--SELECT * FROM [dbo].[vw_JobSchedules];
	--SELECT * FROM [dbo].[Schedule];

	-- Configure Production
	IF (PARSENAME(REPLACE(@@SERVERNAME,'-','.'),3) in ('sdw','mo'))
	BEGIN
		UPDATE S
		  SET [Enabled] = 0
		FROM [dbo].[Schedule] S
		INNER JOIN [Collection] C
			ON S.[CollectionId] = C.[CollectionId]
		WHERE C.[Name] in ('REF-ULN','REF-FCS', 'REF-EPA')
		  AND [Enabled] = 1;

		RAISERROR('		DISABLED : %i Schedule - They should be turned off on this server : %s.',10,1,@@ROWCOUNT,@@SERVERNAME) WITH NOWAIT;
	END

--ROLLBACK
