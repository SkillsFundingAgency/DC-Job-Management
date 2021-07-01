CREATE PROCEDURE GetJobsThatAreProcessingDetailCurrentPeriod
    @JobStatus SMALLINT,
	@NOWUTC DATETIME
AS
BEGIN	

	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 1

	SELECT j.JobId as JobId,
        j.Ukprn as Ukprn,
        c.CollectionYear as CollectionYear,
        f.FileName as FileName,
        DATEDIFF(s, COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC),  j.DateTimeUpdatedUTC) as ProcessingTimeSeconds,
		CT.[Type] as CollectionType
	FROM dbo.Job j 
	INNER JOIN dbo.FileUploadJobMetaData f ON f.JobId = j.JobId
	INNER JOIN dbo.Collection c ON c.CollectionId = j.CollectionId
	INNER JOIN dbo.CollectionType ct ON ct.CollectionTypeId = c.CollectionTypeId
	LEFT JOIN(SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH(NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
	INNER JOIN @YEARSANDPERIODS yp ON yp.[COLLECTIONTYPE] = ct.Type AND yp.[PERIOD] = f.[PeriodNumber] AND yp.[YEAR] = c.[CollectionYear]
	WHERE j.Status = 4 
	ORDER BY j.JobId DESC
 END
GO
 