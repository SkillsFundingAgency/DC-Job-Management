CREATE PROCEDURE [dbo].[GetJobsThatAreSlowFiles]
	@NOWUTC DATETIME
AS
BEGIN
	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 0
	
	SELECT 
	j.JobId as JobId,
	c.CollectionYear as CollectionYear,
	ct.Type as CollectionType,
	AVGG.Ukprn as Ukprn, 
	coalesce(ncs.TouchpointId, fl.FileName) as FileName,
	DATEDIFF(SECOND, J.DateTimeCreatedUTC, @NOWUTC) TimeTakenSecond,
	avgg.AverageTimeSecond AverageTimeSecond
    FROM (
        SELECT 
			INN.CollectionId,
			INN.UKPRN,
			AVG(DateDiffer) AverageTimeSecond,
			AVG(DateDiffer) * 1.2 DateDifferLimit 
			FROM (
	            SELECT 
					j.[Ukprn], 
					j.CollectionId,
                DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) DateDiffer,
                ROW_NUMBER() OVER (PARTITION BY Ukprn, j.CollectionId ORDER BY COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) DESC) AS Rank  
                FROM [dbo].[Job] j WITH (NOLOCK)
                LEFT JOIN (SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH (NOLOCK) GROUP BY JobId) ijmd ON ijmd.JobId = j.JobId
                INNER JOIN [dbo].[Collection] c WITH (NOLOCK)  ON c.CollectionId = j.CollectionId
                inner join dbo.CollectionType ct WITH (NOLOCK)  ON ct.CollectionTypeId = c.CollectionTypeId
				inner join @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND c.CollectionYear = yp.YEAR
                Where j.[Status] = 4 
        ) inn
        WHERE inn.Rank < 4
        GROUP BY inn.Ukprn,
		inn.CollectionId
    ) avgg
    INNER JOIN dbo.Job j WITH (NOLOCK) ON j.Ukprn = avgg.Ukprn and j.CollectionId = avgg.CollectionId
	LEFT JOIN (SELECT JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData] WITH (NOLOCK) GROUP BY JobId) ijmd ON ijmd.JobId = j.JobId
    LEFT JOIN [dbo].FileUploadJobMetaData FL (NOLOCK) ON j.JobId = FL.JobId
	LEFT JOIN [dbo].NcsJobMetaData ncs (NOLOCK) ON j.JobId = ncs.JobId
	inner join dbo.Collection c on j.CollectionId = c.CollectionId
	inner join dbo.CollectionType ct on c.CollectionTypeId = ct.CollectionTypeId
	inner join @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND c.CollectionYear = yp.YEAR
    WHERE j.[Status] IN (2,3) AND DATEDIFF([SECOND], COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) > DateDifferLimit
END
GO
