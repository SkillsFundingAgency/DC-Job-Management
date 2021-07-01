CREATE PROCEDURE [dbo].[GetProvidersReturnedCurrentPeriod]
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
		j.JobId as JobId,
		fujm.FileName as Filename,
		c.CollectionYear as CollectionYear,
		j.[DateTimeCreatedUTC] as DateTimeSubmission,
		DATEDIFF(s, COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC),  j.DateTimeUpdatedUTC) as ProcessingTime
    FROM [dbo].[Job] j (NOLOCK)
		INNER JOIN 
			(SELECT Ukprn, Max([DateTimeCreatedUTC]) as DateTimeCreatedUTC
			FROM dbo.Job j2
			INNER JOIN [dbo].[Collection] c2 on c2.CollectionId = j2.CollectionId
			INNER JOIN [dbo].[CollectionType] ct2 on ct2.CollectionTypeId = c2.CollectionTypeId
			INNER JOIN [dbo].[FileUploadJobMetaData] (NoLock) fujm2 on fujm2.JobId = j2.JobId
			INNER JOIN @yearsAndPeriods yp on yp.[Period] = fujm2.[PeriodNumber] and yp.[Year] = c2.[CollectionYear]
			GROUP BY Ukprn,
				ct2.[Type],
				j2.Status
			HAVING ct2.[Type]='ILR'
				AND j2.Status = 4) groupedj
		ON j.Ukprn = groupedj.Ukprn AND j.DateTimeCreatedUTC = groupedj.DateTimeCreatedUTC
		INNER JOIN [dbo].[Collection] c on c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct on ct.CollectionTypeId = c.CollectionTypeId
		INNER JOIN [dbo].[FileUploadJobMetaData] (NoLock) fujm on fujm.JobId = j.JobId
		INNER JOIN @yearsAndPeriods yp on yp.[Period] = fujm.[PeriodNumber] and yp.[Year] = c.[CollectionYear]
		LEFT JOIN(SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH(NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
	GROUP BY
		j.Ukprn,
		j.JobId,
		fujm.FileName,
		c.CollectionYear,
		j.DateTimeCreatedUTC,
		j.DateTimeUpdatedUTC,
		ct.[Type],
		j.Status,
		ijmd.DateTimeSubmittedUtc
	HAVING
	 ct.[Type] = 'ILR' 
	 AND j.Status = 4
	
RETURN 0