CREATE PROCEDURE GetJobsThatAreProcessingDetailCurrentPeriodLast5Mins
    @JobStatus SMALLINT,
	@NOWUTC DATETIME
AS
BEGIN	
    DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 1
  
    SELECT 
        j.JobId as JobId,
        j.Ukprn as Ukprn,
        c.CollectionYear as CollectionYear,
        coalesce(ncs.TouchpointId, fujm.FileName) as FileName,
        DATEDIFF(s, COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC),  j.DateTimeUpdatedUTC) as ProcessingTimeSeconds,
        CT.[Type] as CollectionType
    FROM [dbo].[Job] j (NOLOCK)
    INNER JOIN [dbo].[Collection] c (NOLOCK) ON j.CollectionId = c.CollectionId
    INNER JOIN [dbo].[CollectionType] ct (NOLOCK) on c.CollectionTypeId = ct.CollectionTypeId
    LEFT JOIN [dbo].[FileUploadJobMetaData] fujm (NOLOCK) on j.JobId = fujm.JobId
    LEFT JOIN [dbo].[NcsJobMetaData] ncs (NOLOCK) on j.JobId = ncs.JobId
    LEFT JOIN(SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH(NOLOCK) GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
    INNER JOIN @YEARSANDPERIODS yp ON yp.[COLLECTIONTYPE] = ct.Type AND yp.[PERIOD] = coalesce(ncs.PeriodNumber, fujm.[PeriodNumber]) AND yp.[YEAR] = c.[CollectionYear]
    WHERE 
        j.[Status] = @JobStatus AND
        j.DateTimeUpdatedUTC BETWEEN DATEADD(mi, -5, @NOWUTC) AND @NOWUTC
END
GO
