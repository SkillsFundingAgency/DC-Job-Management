SET NOCOUNT ON

DECLARE @SummaryOfChanges_EmailValidityPeriods TABLE ([HubEmailId] INT, [HubPathItemId] INT, CollectionYear VARCHAR(50), PeriodNumbers VARCHAR(50), [Action] VARCHAR(100));

DECLARE @listOfPeriods TABLE([period] TINYINT)
INSERT @listOfPeriods([period]) VALUES(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)

DECLARE @listOfYears TABLE([year] INT)
INSERT @listOfYears([year]) VALUES(1920),(2021)

DECLARE @listOfEmails TABLE([hubEmailId] INT, [HubPathItemId] INT, [email] NVARCHAR(200))
INSERT @listOfEmails([hubEmailId], [HubPathItemId], [email]) VALUES
	(3, 13, 'Data Warehouse 1 Email'),
	(4, 3, 'DAS Started Email'),
	(5, 26, 'FCS Handover Part 1 Email'),

	(7, 35, 'Standard File Email'),

	(9, 44, 'Data Warehouse 2 Email'),

	(12, 27, 'FCS Team Handover Email'),
	(13, 0, 'MCA Reports Available Email'),
	(14, 101, 'NCS FCS Handover Email'),

	(17, 110, 'FCS Handover Part 2 Email'),

	(18, 113, 'Data Quality Report Email'),
	(19, 114, 'Provider Submissions Report Email'),
	(20, 115, 'Period End ESFA Non-Contracted Devolved Adult Education Activity Report Email'),
	(21, 116, 'Data Export Report Email'),
	(22, 120, 'Reports Available Email')


DECLARE @validityPeriods TABLE ([HubEmailId] INT, [HubPathItemId] INT, [CollectionYear] VARCHAR(50), [Period] VARCHAR(50))
INSERT @validityPeriods
	SELECT [emails].[HubEmailId], [emails].[HubPathItemId], [years].[year], [periods].[period]
	FROM @listOfEmails [emails]
	CROSS JOIN @listOfPeriods [periods]
	CROSS JOIN @listOfYears [years]

MERGE INTO [PeriodEnd].[EmailValidityPeriod] AS TARGET
			USING (
				SELECT [hubEmailId], [HubPathItemId], [CollectionYear], [Period]
				FROM @validityPeriods
			)
			AS SOURCE([HubEmailId], [HubPathItemId], [CollectionYear], [Period])
			ON TARGET.[HubEmailId] = SOURCE.[HubEmailId]
				AND TARGET.[Period] = SOURCE.[Period]
				AND TARGET.[CollectionYear] = SOURCE.[CollectionYear]

			WHEN NOT MATCHED BY TARGET THEN 
					INSERT([HubEmailId],[HubPathItemId],[Period],[CollectionYear])
						VALUES ([HubEmailId],[HubPathItemId],[Period],[CollectionYear])
			WHEN NOT MATCHED BY SOURCE THEN DELETE

			OUTPUT Inserted.[HubEmailId], Inserted.[HubPathItemId], Inserted.[CollectionYear], Inserted.[Period], $action 
				INTO @SummaryOfChanges_EmailValidityPeriods([HubEmailId], [HubPathItemId], [CollectionYear], [PeriodNumbers], [Action])
			;

DECLARE @AddCount_CT INT, @DeleteCount_CT INT

SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_EmailValidityPeriods WHERE [Action] = 'Insert' GROUP BY Action),0);
SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_EmailValidityPeriods WHERE [Action] = 'Delete' GROUP BY Action),0);

RAISERROR('		        %s - Added %i - Delete %i',10,1,'     EmailValidityPeriod', @AddCount_CT, @DeleteCount_CT) WITH NOWAIT;

GO