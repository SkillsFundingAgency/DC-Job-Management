CREATE PROCEDURE GetJobsThatAreSubmitted
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
           J.DateTimeCreatedUTC CreatedDate, 
           J.Status, 
           ST.StatusDescription StatusDescription, 
           CT.[Type] CollectionType, 
           coalesce(ncs.TouchpointId, FL.FileName) FileName
	from [dbo].[Job] J (NoLock)
    left join (Select JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc from [dbo].[IlrJobMetaData] (NoLock) Group by jobid) ijmd on ijmd.JobId = j.JobId
    inner join [dbo].[Collection] c on c.CollectionId = j.CollectionId
    inner join [dbo].[CollectionType] ct on ct.CollectionTypeId = c.CollectionTypeId
	inner join [dbo].JobStatusType ST (NOLOCK) ON J.Status = ST.StatusId
	inner join @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND c.CollectionYear = yp.YEAR
	left join [dbo].FileUploadJobMetaData FL (NOLOCK) ON J.JobId = FL.JobId
	left join [dbo].[NcsJobMetaData] ncs (NOLOCK) ON J.JobId = ncs.JobId
    where 
    coalesce(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @FROMMIDNIGHTUTC and
    (ct.CollectionTypeId <> 1 or ijmd.DateTimeSubmittedUtc is not null)
END
GO
