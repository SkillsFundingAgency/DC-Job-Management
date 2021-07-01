CREATE PROCEDURE [dbo].[GetILRSubmissionsPerDay]
(
    @collectionYear INT,
	@periodNumber INT
)
AS
BEGIN

	DECLARE @CollectionId INT;
	DECLARE @EndDateOfCollection DATE

	SELECT 
		@CollectionId = CollectionId 
	FROM Collection 
	WHERE 
		CollectionTypeId = 1 AND 
		CollectionYear = @collectionYear


	SELECT @EndDateOfCollection = CAST(rp.EndDateTimeUTC as DATE)
	FROM
		[dbo].[ReturnPeriod] rp
	WHERE 
		rp.CollectionId = @CollectionId AND 
		rp.PeriodNumber = @periodNumber

	SELECT
		COUNT(j.Ukprn) NumberOfSubmissions, 
		CAST(j.DateTimeCreatedUTC AS DATE) DateTimeCreatedUTC,
		DATEDIFF(DAY,  @EndDateOfCollection, CAST(j.DateTimeCreatedUTC AS DATE)) DaysToClose		
	FROM [dbo].[Job] j
			INNER JOIN [dbo].[FileUploadJobMetaData] fujm ON j.JobId = fujm.JobId
	WHERE 
		j.Status = 4 and 
		j.CollectionId = @CollectionId and
		fujm.PeriodNumber = @periodNumber
	GROUP BY CAST(j.DateTimeCreatedUTC AS DATE)
	ORDER BY cast(j.DateTimeCreatedUTC AS DATE) ASC


END
GO
GRANT EXECUTE ON [dbo].[GetILRSubmissionsPerDay] TO [DataViewer];
GO
