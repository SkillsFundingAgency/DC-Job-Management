SET NOCOUNT ON

DECLARE @SummaryOfChanges_ValidityPeriods TABLE ([HubPathId] INT, CollectionYear VARCHAR(50), PeriodNumbers VARCHAR(50), [Action] VARCHAR(100));

DECLARE @listOfYears TABLE([year] INT)
INSERT @listOfYears([year]) VALUES(1920),(2021)

DECLARE @listOfPeriods TABLE([period] TINYINT)
INSERT @listOfPeriods([period]) VALUES(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)

DECLARE @listOfPaths TABLE([HubPathId] INT, [name] NVARCHAR(200))
INSERT @listOfPaths([HubPathId], [name]) VALUES
	(1, 'DataWarehouse1Path'),
	(2, 'DASStartedPath'),
	(4, 'DataWarehouse2Path'), 
	(5, 'InternalReportsPath'), 
	(6, 'FCSHandOverPath2'), 
	(8, 'ReferenceDataPath'),
	(9, 'NCSCriticalPath'),
	(10, 'ALLFCriticalPath'),
	(11, 'FCSHandOverPath1')
	
DECLARE @validityPeriods TABLE ([HubPathId] INT, [CollectionYear] VARCHAR(50), [Period] VARCHAR(50))

INSERT @validityPeriods
	SELECT paths.HubPathId, [years].[year], [periods].[period]
	FROM @listOfPaths paths
	CROSS JOIN @listOfPeriods [periods]
	CROSS JOIN @listOfYears [years]


MERGE INTO [PeriodEnd].[SubPathValidityPeriod] AS TARGET
			USING (
				SELECT [HubPathId], [CollectionYear], [Period]
				FROM @validityPeriods
			)
			AS SOURCE([HubPathId], [CollectionYear], [Period])
			ON TARGET.[HubPathId] = SOURCE.[HubPathId]
				AND TARGET.[Period] = SOURCE.[Period]
				AND TARGET.[CollectionYear] = SOURCE.[CollectionYear]

			WHEN NOT MATCHED BY TARGET THEN 
					INSERT([HubPathId],[Period],[CollectionYear])
						VALUES ([HubPathId],[Period],[CollectionYear])
			WHEN NOT MATCHED BY SOURCE THEN DELETE

			OUTPUT Inserted.[HubPathId], Inserted.[CollectionYear], Inserted.[Period], $action 
				INTO @SummaryOfChanges_ValidityPeriods([HubPathId], [CollectionYear], [PeriodNumbers], [Action])
			;

DECLARE @AddCount_CT INT, @DeleteCount_CT INT

SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidityPeriods WHERE [Action] = 'Insert' GROUP BY Action),0);
SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidityPeriods WHERE [Action] = 'Delete' GROUP BY Action),0);

RAISERROR('		        %s - Added %i - Delete %i',10,1,'PeriodEnd.SubPathValidityPeriod', @AddCount_CT, @DeleteCount_CT) WITH NOWAIT;

GO