CREATE PROCEDURE GetJobsThatAreQueued
	@NOWUTC DATETIME
AS
BEGIN  
	Declare @nowUtcToday DateTime = DATEADD(day, DATEDIFF(day, 0, @NOWUTC), 0)

	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @nowUtcToday, @collectiontypes = @json, @includePeriodEnd = 1

	SELECT 
		c.CollectionYear,
		J.JobId, 
		   J.Ukprn, 
		   DATEDIFF(SECOND, J.DateTimeCreatedUTC, @NOWUTC) TimeInQueueSecond, 
		   CT.[Type] CollectionType, 
		   J.Status, 
		   ST.StatusDescription StatusDescription
	FROM Job J
	LEFT JOIN (SELECT JobId, MAX(DateTimeSubmittedUtc) DateTimeSubmittedUtc FROM dbo.[IlrJobMetaData] (NOLOCK) GROUP BY JobId) IJMD ON IJMD.JobId = J.JobId
	INNER JOIN dbo.Collection C (NOLOCK) ON J.CollectionId = C.CollectionId
	INNER JOIN dbo.CollectionType CT (NOLOCK) ON C.CollectionTypeId = CT.CollectionTypeId
	INNER JOIN dbo.JobStatusType ST (NOLOCK) ON J.Status = ST.StatusId
	INNER JOIN @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND yp.YEAR = c.CollectionYear
	WHERE J.Status = 1 AND 
		  (CT.CollectionTypeId <> 1 OR IJMD.DateTimeSubmittedUtc IS NOT NULL) AND 
		  coalesce(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @nowUtcToday
END
GO
