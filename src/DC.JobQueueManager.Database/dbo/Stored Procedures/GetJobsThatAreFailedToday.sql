CREATE PROCEDURE GetJobsThatAreFailedToday
	@FROMMIDNIGHTUTC DATETIME
AS
BEGIN  
	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @FROMMIDNIGHTUTC, @collectiontypes = @json, @includePeriodEnd = 1

	Select 
		c.CollectionYear,
		J.JobId, 
		J.Ukprn,
		j.DateTimeUpdatedUTC FailedAt,
		DATEDIFF(SECOND, coalesce(ijmd.DateTimeSubmittedUtc, J.DateTimeCreatedUTC), j.DateTimeUpdatedUTC) ProcessingTimeBeforeFailureSecond, 
		coalesce(ncs.TouchpointId, fl.FileName) Filename,
		CT.[Type] as CollectionType
    from [dbo].[Job] J (NoLock)
    left join (Select JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc from [dbo].[IlrJobMetaData] (NoLock) Group by JobId) ijmd on ijmd.JobId = j.JobId
    inner join [dbo].[Collection] c on c.CollectionId = j.CollectionId
    inner join [dbo].[CollectionType] ct on ct.CollectionTypeId = c.CollectionTypeId
	INNER JOIN @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND yp.YEAR = c.CollectionYear
	left join [dbo].[FileUploadJobMetaData] fl (nolock) on j.JobId = fl.JobId
	left join [dbo].[NcsJobMetaData] ncs (nolock) on j.JobId = ncs.JobId
    where 
    coalesce(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @FROMMIDNIGHTUTC and
    (ct.CollectionTypeId <> 1 or ijmd.DateTimeSubmittedUtc is not null) and
	j.Status in (5, 6)
END
GO
