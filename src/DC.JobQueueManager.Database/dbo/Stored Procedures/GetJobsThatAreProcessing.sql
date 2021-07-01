CREATE PROCEDURE GetJobsThatAreProcessing @NOWUTC DATETIME
AS
	BEGIN
		DECLARE @nowUtcToday DATETIME = DATEADD(DAY, DATEDIFF(DAY, 0, @NOWUTC), 0);
		DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
		DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

		INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
		EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 1

		SELECT j.JobId, j.Ukprn, DATEDIFF(SECOND, j.DateTimeCreatedUTC, @NOWUTC) TimeTakenSecond, COALESCE([avg].DateDifferSecond, 0) DateDifferSecond, ct.[Type] CollectionType, j.Status, ST.StatusDescription StatusDescription
		FROM [dbo].[Job] j (NOLOCK)
			  LEFT JOIN (SELECT JobId, MAX(DateTimeSubmittedUtc) DateTimeSubmittedUtc
							 FROM [dbo].[IlrJobMetaData] (NOLOCK)
							 GROUP BY jobid) ijmd ON ijmd.JobId = j.JobId
			  INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
			  INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
			  INNER JOIN @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND yp.YEAR = c.CollectionYear
			  INNER JOIN dbo.JobStatusType ST (NOLOCK) ON j.Status = ST.StatusId
			  LEFT JOIN (
						SELECT INN.CollectionYear, Ukprn, CollectionId, AVG(DATEDIFFERSECOND) DateDifferSecond
							 FROM (
							 SELECT C.CollectionYear, J.Ukprn, J.CollectionId, DATEDIFF(SECOND, COALESCE(IJMD.DateTimeSubmittedUtc, J.DateTimeCreatedUTC), J.DateTimeUpdatedUTC) DATEDIFFERSECOND, 
										ROW_NUMBER() OVER (PARTITION BY Ukprn, J.CollectionId ORDER BY COALESCE(IJMD.DateTimeSubmittedUtc, J.DateTimeCreatedUTC) DESC) AS RANK
									 FROM [dbo].[Job] J (NOLOCK)
											LEFT JOIN (SELECT JobId, MAX(DateTimeSubmittedUtc) DateTimeSubmittedUtc
														  FROM [dbo].[IlrJobMetaData] (NOLOCK)
														  GROUP BY JobId) IJMD ON IJMD.JobId = J.JobId
											INNER JOIN [dbo].[Collection] C ON C.CollectionId = J.CollectionId
											INNER JOIN [dbo].[CollectionType] ct ON c.CollectionTypeId = ct.CollectionTypeId
											INNER JOIN @YEARSANDPERIODS yp on yp.COLLECTIONTYPE = ct.Type AND yp.YEAR = c.CollectionYear
									 WHERE J.Status = 4) INN
							 WHERE INN.RANK < 4
							 GROUP BY INN.CollectionYear, INN.Ukprn, INN.[CollectionId] ) [avg] ON j.Ukprn = [avg].Ukprn
		WHERE COALESCE(ijmd.DateTimeSubmittedUtc, j.DateTimeCreatedUTC) > @nowUtcToday
			AND (ct.CollectionTypeId <> 1
					OR ijmd.DateTimeSubmittedUtc IS NOT NULL)
			AND j.Status IN (2, 3)
			AND avg.CollectionId = j.CollectionId;
	END;
GO


