CREATE PROCEDURE [dbo].[Dashboard]
AS
  
    DECLARE @nowUtc DATETIME = GETUTCDATE()
    DECLARE @nowUtcToday DATETIME = DATEADD(day, DATEDIFF(day, 0, @nowUtc), 0)
    DECLARE @nowUtcLastHour DATETIME = DATEADD(hh, -1, @nowUtc)
    DECLARE @nowUtcLast5Minutes DATETIME = DATEADD(mi, -5, @nowUtc)

    DECLARE @periodNumber INT
    DECLARE @collectionYear INT
	DECLARE @collectionType VARCHAR(50)
    DECLARE @concerns TABLE (
		[JobId] BIGINT,
        [CollectionYear] INT,
        [Ukprn] BIGINT,
        [FileName] VARCHAR(100),
        [LastSuccessfulSubmission] DATETIME,
        [PeriodOfLastSuccessfulSubmission] INT,
		[CollectionType] varchar(50)
    )
    
	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 1

    DECLARE Period_Cursor CURSOR FORWARD_ONLY FOR  
    SELECT [COLLECTIONTYPE], [PERIOD], [YEAR]
    FROM @YEARSANDPERIODS

    OPEN Period_Cursor;  
    FETCH NEXT FROM Period_Cursor
    INTO @collectionType, @PeriodNumber, @CollectionYear;  
    WHILE @@FETCH_STATUS = 0  
       BEGIN
            Insert into @concerns
			EXEC [dbo].[GetJobConcernsPerCollectionPerPeriod] @collectionType, @collectionYear, @periodNumber

            FETCH NEXT FROM Period_Cursor
            INTO @collectionType, @PeriodNumber, @CollectionYear;
       END;  
    CLOSE Period_Cursor;  
    DEALLOCATE Period_Cursor;
	
	SELECT
		COALESCE(AVG(DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC)), 0) AverageTimeToday,
		COALESCE(
				AVG( 
						CASE WHEN DateTimeUpdatedUTC >= @nowUtcLastHour 
							THEN DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) 
							ELSE NULL 
						END
					), 0
				) AverageTimeLastHour,
		COUNT(CASE j.[Status] WHEN 2 THEN 1 WHEN 3 THEN 1 ELSE NULL END) JobsProcessing,
		Count(case j.[Status] WHEN 1 THEN 1 ELSE NULL END) JobsQueued,
		Count(case j.[Status] WHEN 5 THEN 1 WHEN 6 THEN 1 ELSE NULL END) FailedToday,
		Count(j.JobId) SubmissionsToday,
		Count(CASE WHEN j.[Status] = 4 AND  j.DateTimeUpdatedUTC >= @nowUtcLastHour AND j.DateTimeUpdatedUTC <= @nowUtc THEN 1 ELSE Null END) SubmissionsLastHour,
		Count(CASE WHEN j.[Status] = 4 AND  j.DateTimeUpdatedUTC >= @nowUtcLast5Minutes AND j.DateTimeUpdatedUTC <= @nowUtc THEN 1 ELSE Null END) SubmissionsLast5Minutes,
		C.CollectionYear CollectionYear
	FROM [dbo].[Job] J WITH (NOLOCK)
		LEFT JOIN(SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH(NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
		INNER JOIN [dbo].[Collection] c WITH (NOLOCK) ON c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK) ON ct.CollectionTypeId = c.CollectionTypeId
		INNER JOIN @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type and c.CollectionYear = yp.YEAR
	WHERE 
		COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @nowUtcToday AND
		(ct.CollectionTypeId <> 1 OR ijmd.DateTimeSubmittedUtc IS NOT NULL)
	GROUP BY c.CollectionYear
	
	SELECT
		COALESCE(AVG(DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC)), 0) AverageTimeToday,
		COALESCE(
				AVG( 
						CASE WHEN DateTimeUpdatedUTC >= @nowUtcLastHour 
							THEN DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) 
							ELSE NULL 
						END
					), 0
				) AverageTimeLastHour
    FROM [dbo].[Job] J WITH (NOLOCK)
		LEFT JOIN(SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH (NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
		INNER JOIN [dbo].[Collection] c WITH (NOLOCK) ON c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK) ON ct.CollectionTypeId = c.CollectionTypeId
    WHERE 
    COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @nowUtcToday AND
    ct.[Type] NOT IN ('REF') AND 
    (ct.CollectionTypeId <> 1 OR ijmd.DateTimeSubmittedUtc IS NOT NULL)

	SELECT COUNT(avgg.Ukprn) as SlowFilesComparedToThreePrevious
    FROM (
        SELECT Ukprn, CollectionId, AVG(DateDiffer) * 1.2 DateDifferLimit FROM (
            SELECT j.[Ukprn], 
                    j.CollectionId,
                    DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) DateDiffer,
                    ROW_NUMBER() OVER (PARTITION BY Ukprn, j.CollectionId ORDER BY COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) DESC) AS Rank  
                FROM [dbo].[Job] j WITH (NOLOCK)
                LEFT JOIN (SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH (NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
                INNER JOIN [dbo].[Collection] c WITH (NOLOCK)  ON c.CollectionId = j.CollectionId
                inner join dbo.CollectionType ct WITH (NOLOCK)  ON ct.CollectionTypeId = c.CollectionTypeId
                Where j.[Status] = 4 
				AND c.CollectionYear IN (SELECT [YEAR] FROM @YEARSANDPERIODS yp WHERE yp.COLLECTIONTYPE = ct.Type)
                AND ct.Type in ('ILR','NCS','ESF','EAS')
        ) inn
        WHERE inn.Rank < 4
        GROUP BY inn.Ukprn, inn.CollectionId
    ) avgg
    INNER JOIN dbo.Job j WITH (NOLOCK) ON j.Ukprn = avgg.Ukprn and j.CollectionId = avgg.CollectionId
    LEFT JOIN (SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH (NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
    WHERE j.[Status] IN (2,3) AND DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) > DateDifferLimit

  	SELECT 
		COUNT(CASE j.[Status] WHEN 4 THEN 1 ELSE null END) JobsCurrentPeriod, 
		COUNT(CASE j.[Status] WHEN 5 THEN 1 WHEN 6 THEN 1 ELSE null END) JobsFailedInPeriod, 
		yp.YEAR CollectionYear, 
		yp.[PERIOD] PeriodNumber
    FROM [dbo].[Job] j WITH (NOLOCK)
		INNER JOIN [dbo].[Collection] c WITH (NOLOCK) ON c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK)  ON ct.CollectionTypeId = c.CollectionTypeId
		INNER JOIN [dbo].[FileUploadJobMetaData] ijmd WITH (NOLOCK) ON ijmd.JobId = j.JobId
		INNER JOIN @YEARSANDPERIODS yp ON yp.COLLECTIONTYPE = ct.Type AND yp.[PERIOD] = ijmd.[PeriodNumber] AND yp.[YEAR] = c.[CollectionYear]
    GROUP BY yp.YEAR, yp.[PERIOD]

	SELECT 
		COUNT(DISTINCT j.Ukprn) CountOfSuccessfulIlrProvidersInPeriod, 
		yp.YEAR CollectionYear, 
		yp.[PERIOD] PeriodNumber
    FROM [dbo].[Job] j WITH (NOLOCK)
		INNER JOIN [dbo].[Collection] c WITH (NOLOCK) ON c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK) ON ct.CollectionTypeId = c.CollectionTypeId
		INNER JOIN [dbo].[FileUploadJobMetaData] ijmd WITH (NOLOCK) ON ijmd.JobId = j.JobId
		INNER JOIN @YEARSANDPERIODS yp ON yp.COLLECTIONTYPE = ct.Type AND yp.[PERIOD] = ijmd.[PeriodNumber] AND yp.[YEAR] = c.[CollectionYear]
	WHERE ct.[Type] = 'ILR' AND j.Status = 4
    GROUP BY yp.YEAR, yp.[PERIOD]

	SELECT
		j.Ukprn AS Ukprn,
        j.JobId,
		yp.YEAR CollectionYear, 
		yp.[PERIOD] PeriodNumber
	FROM [dbo].[Job] j WITH (NoLock)
	INNER JOIN [dbo].[FileUploadJobMetaData] fujm WITH (NOLOCK) ON j.JobId = fujm.JobId
	INNER JOIN [dbo].[Collection] c WITH (NOLOCK) ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK) ON ct.CollectionTypeId = c.CollectionTypeId
	INNER JOIN @YEARSANDPERIODS yp ON yp.COLLECTIONTYPE = ct.Type AND yp.[PERIOD] = fujm.[PeriodNumber] AND yp.[YEAR] = c.[CollectionYear]
	WHERE ct.[Type] = 'ILR' AND j.Status = 4

    SELECT COUNT(*) Concerns FROM @concerns
	
    SELECT DISTINCT [YEAR] from @YEARSANDPERIODS

RETURN 0