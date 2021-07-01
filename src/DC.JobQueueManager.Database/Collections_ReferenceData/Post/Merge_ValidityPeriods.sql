SET NOCOUNT ON
RAISERROR('		   Validity Period Merge',10,1) WITH NOWAIT;

-- Validity items
DECLARE @PEAppSummarisation int = 24
DECLARE @PECollectionStats int = 8
DECLARE @PEDASAppsAdditionalPaymentsReport int = 37
DECLARE @PEDASAppsCoInvestmentContributionsReport int = 38
DECLARE @PEDASAppsDataMatchMonthEndReport int = 41
DECLARE @PEDASAppsInternalDataMatchMonthEndReport int = 31
DECLARE @PEDASAppsMonthlyPaymentReport int = 39
DECLARE @PEDASCrossYearPaymentsReport int = 40
DECLARE @PEDASPeriodEndReportPreparation int = 36
DECLARE @PEDASReportsFinished int = 48
DECLARE @PEDASRun int = 22
DECLARE @PEDASStart int = 10
DECLARE @PEDASStop int = 46
DECLARE @PEDASSubmission int = 9
DECLARE @PEDASSubmissionWindowPeriodValidation int = 103
DECLARE @PEDataExtractReport int = 28
DECLARE @PEDataQualityReport int = 5
DECLARE @PEDataWarehouse1 int = 12
DECLARE @PEDataWarehouse2 int = 43
DECLARE @PEDCSummarisation int = 19
DECLARE @PEESFANonContDevolvedAdultEducationReport int = 50
DECLARE @PEESFSummarisation int = 20
DECLARE @PEMCAReports int = 21
DECLARE @PEMetricsReport int = 23
DECLARE @PEProviderSubmissionsReport int = 6
DECLARE @PEStandardFile int = 34
DECLARE @PEUYPLLVReport int = 49
DECLARE @REFEPA int = 16
DECLARE @REFFCS int = 14
DECLARE @DASReprocess int = 112
DECLARE @CrossYearIndicativePaymentsReport int = 111
DECLARE @PENCSDataExtractReport int = 102
DECLARE @PENCSSummarisation int = 100
DECLARE @ALLFSummarisation int = 104
DECLARE @PEACTCountReport int = 32

-- Validity tables
DECLARE @validityPeriods TABLE ([HubPathItemId] INT, [CollectionYear] INT, [Period] INT)
DECLARE @SummaryOfChanges_ValidityPeriods TABLE ([HubPathItemId] INT, [CollectionYear] VARCHAR(50), [Period] VARCHAR(50), [Action] VARCHAR(100));

-- Standard Periods
DECLARE @StandardValidityPeriods TABLE([period] TINYINT)
INSERT @StandardValidityPeriods([period])
VALUES(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)

-- Supported validity years
DECLARE @ValidityYears TABLE([year] INT)
INSERT @ValidityYears([year])
VALUES(1920), (2021), (2122)

DECLARE @ValidityPathItems TABLE([pathItem] INT)
INSERT @ValidityPathItems([pathItem])
VALUES (@PEAppSummarisation), 
(@PECollectionStats), 
(@PEDASAppsAdditionalPaymentsReport),
(@PEDASAppsCoInvestmentContributionsReport),
(@PEDASAppsDataMatchMonthEndReport),
(@PEDASAppsInternalDataMatchMonthEndReport), 
(@PEDASAppsMonthlyPaymentReport),
(@PEDASCrossYearPaymentsReport), 
(@PEDASPeriodEndReportPreparation), 
(@PEDASReportsFinished), 
(@PEDASRun),
(@PEDASStart),
(@PEDASStop),
(@PEDASSubmission),
(@PEDASSubmissionWindowPeriodValidation),
(@PEDataExtractReport),
(@PEDataQualityReport),
(@PEDataWarehouse1),
(@PEDataWarehouse2),
(@PEDCSummarisation),
(@PEESFANonContDevolvedAdultEducationReport),
(@PEESFSummarisation),
(@PEMCAReports),
(@PEMetricsReport),
(@PEProviderSubmissionsReport),
(@PEStandardFile),
(@PEUYPLLVReport),
(@REFEPA),
(@REFFCS),
(@DASReprocess),
(@CrossYearIndicativePaymentsReport),
(@PENCSDataExtractReport),
(@PENCSSummarisation),
(@PEACTCountReport)

INSERT INTO @validityPeriods ([HubPathItemId], [CollectionYear], [Period])
SELECT PathItem,  years.year, periods.period
FROM @ValidityPathItems
CROSS JOIN @StandardValidityPeriods [periods]
CROSS JOIN @ValidityYears [years]

-- Remove invalid 1920 items
DELETE @validityPeriods
WHERE CollectionYear = 1920
AND HubPathItemId IN (@CrossYearIndicativePaymentsReport, @PENCSDataExtractReport, @PENCSSummarisation)

 --- ALLF -- 
DECLARE @ALLFValidityPeriods TABLE([period] TINYINT)
INSERT @ALLFValidityPeriods([period])
VALUES(55),(56),(57),(58),(59),(60),(61),(62),(63),(64),(65),(66),(67),(68),(69),(70),(71),(72),(73),(74),(75),(76),(77),(78),(79),(80),(81),(82),(83),(84)

DECLARE @ALLFValidityPathItems TABLE([pathItem] INT)
INSERT @ALLFValidityPathItems([pathItem])
VALUES (@ALLFSummarisation)

INSERT INTO @validityPeriods ([HubPathItemId], [CollectionYear], [Period])
SELECT PathItem,  0, periods.period
FROM @ALLFValidityPathItems
CROSS JOIN @ALLFValidityPeriods [periods]

-- Commit to DB
MERGE INTO [PeriodEnd].[ValidityPeriod] AS TARGET
			USING (
				SELECT [HubPathItemId], [CollectionYear], [Period]
				FROM @validityPeriods
			)
			AS SOURCE([HubPathItemId], [CollectionYear], [Period])
				ON TARGET.[Period] = SOURCE.[Period]
				AND TARGET.[CollectionYear] = SOURCE.[CollectionYear]
				AND TARGET.[HubPathItemId] = SOURCE.[HubPathItemId]

			WHEN NOT MATCHED BY TARGET THEN 
					INSERT([HubPathItemId], [Period], [CollectionYear])
						VALUES ([HubPathItemId], [Period], [CollectionYear])

			WHEN NOT MATCHED BY SOURCE THEN
				DELETE 

			OUTPUT Inserted.[HubPathItemId], Inserted.[CollectionYear], Inserted.[Period], $action 
				INTO @SummaryOfChanges_ValidityPeriods([HubPathItemId], [CollectionYear], [Period], [Action])
			;

DECLARE @AddCount_ValidityPeriods_CT INT, @DeleteCount_ValidityPeriods_CT INT

SET @AddCount_ValidityPeriods_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidityPeriods WHERE [Action] = 'Insert' GROUP BY Action),0);
SET @DeleteCount_ValidityPeriods_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidityPeriods WHERE [Action] = 'Delete' GROUP BY Action),0);

RAISERROR('		        %s - Added %i - Delete %i',10,1,'PeriodEnd.ValidityPeriods', @AddCount_ValidityPeriods_CT, @DeleteCount_ValidityPeriods_CT) WITH NOWAIT;