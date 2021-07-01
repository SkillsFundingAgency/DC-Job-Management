CREATE PROCEDURE [dbo].[GetSldToDasMismatchDrillDown]
    @period int,
    @year int,
	@ukprnList NVARCHAR(MAX)
AS
	SELECT
		j.Ukprn as Ukprn,
		j.JobId as JobId,
		fujm.FileName as FileName,
		MAX(ijm.DateTimeSubmittedUtc) as DateTimeSubmittedUtc
	FROM [dbo].[Job] j
	INNER JOIN [dbo].[FileUploadJobMetaData] fujm ON j.JobId = fujm.JobId
	LEFT OUTER JOIN [dbo].[IlrJobMetaData] ijm ON j.JobId = ijm.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	GROUP BY
		j.Ukprn,
		j.JobId,
		fujm.FileName,
		ct.[Type],
		j.Status,
		fujm.PeriodNumber,
		c.CollectionYear
	HAVING 
		ct.[Type] = 'ILR' 
		AND j.Status = 4
		AND fujm.PeriodNumber = @period
		AND c.CollectionYear = @year
		AND j.Ukprn in (SELECT CONVERT(NVARCHAR(8),Value) as [Ukprn] FROM OPENJSON(@ukprnList,'$.Ukprn'))
	
RETURN 0