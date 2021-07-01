CREATE PROCEDURE [dbo].[GetIlrJobsSuccessfulInPeriod]
	@collectionYear int = null
AS
	DECLARE @nowUtc DATETIME = GETUTCDATE()

    DECLARE @yearsAndPeriods TABLE (
        [Year] INT,
        [Period] INT
    )

    INSERT INTO @yearsAndPeriods ([Period], [Year])
    SELECT PeriodNumber, CollectionYear  
    FROM (SELECT [StartDateTimeUTC]
        ,LEAD(StartDateTimeUTC, 1, EndDateTimeUTC + 6) OVER (PARTITION BY rp.CollectionId ORDER BY PeriodNumber) [EndDateTimeLeadUTC]
        ,[PeriodNumber]
        ,c.CollectionYear 
        FROM [dbo].[ReturnPeriod] rp
        INNER JOIN [dbo].[Collection] c ON c.CollectionId = rp.CollectionId
        INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
        WHERE ct.[Type] = 'ILR' AND
		(@collectionYear IS NULL OR (c.CollectionYear = @collectionYear))) ilrPeriodsStretched
    WHERE StartDateTimeUTC <= @nowUtc AND
    EndDateTimeLeadUTC >= @nowUtc

	SELECT
		DISTINCT(j.Ukprn) as Ukprn,
		c.CollectionYear,
		yp.[Period] as PeriodNumber,
		yp.[Year] as CollectionYear
	FROM [dbo].[Job] j
	INNER JOIN [dbo].[FileUploadJobMetaData] fujm ON j.JobId = fujm.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	INNER JOIN @yearsAndPeriods yp ON yp.[Period] = fujm.[PeriodNumber] AND yp.[Year] = c.[CollectionYear]
	WHERE ct.[Type] = 'ILR' AND j.Status = 4

RETURN 0