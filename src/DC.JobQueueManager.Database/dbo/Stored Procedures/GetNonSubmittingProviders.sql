CREATE PROCEDURE [dbo].[GetNonSubmittingProviders] 
	@collectionName VARCHAR(50),
	@periodNumber INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @completeStatus INT = 4

	DECLARE @expectedReturners TABLE (UkPrn BIGINT)
	DECLARE @currentPeriodSubmissions TABLE (UkPrn BIGINT)
	DECLARE @latestSubmissions TABLE (JobId BIGINT, UkPrn BIGINT)

	INSERT @expectedReturners
		SELECT o.Ukprn
		FROM Organisation o
		INNER JOIN OrganisationCollection oc ON o.OrganisationId = oc.OrganisationId 
		INNER JOIN Collection c ON oc.CollectionId = c.CollectionId	
		INNER JOIN ReturnPeriod rp ON rp.PeriodNumber = @periodNumber
		WHERE c.Name = @collectionName
		GROUP BY o.Ukprn

	INSERT @currentPeriodSubmissions
		SELECT
			j.Ukprn
		FROM  dbo.Job j
		INNER JOIN Collection c ON c.CollectionId = j.CollectionId
		INNER JOIN FileUploadJobMetaData meta on meta.JobId = j.JobId
		INNER JOIN ReturnPeriod rp ON j.CollectionId = rp.CollectionId AND rp.PeriodNumber = meta.PeriodNumber
		INNER JOIN @expectedReturners er ON er.UkPrn = j.Ukprn
		WHERE j.[Status] = @completeStatus
		AND rp.PeriodNumber = @periodNumber 
		AND c.[Name] = @collectionName
		GROUP BY j.Ukprn
	
	INSERT @latestSubmissions			
		SELECT Submissions.JobId, Submissions.Ukprn FROM 
		(
			SELECT j.JobId, j.CollectionId, j.Ukprn, meta.Id, meta.DateTimeSubmittedUtc ,
					RANK() OVER (PARTITION BY j.Ukprn,j.CollectionId ORDER BY meta.DateTimeSubmittedUtc, meta.Id DESC) AS Rank  
			FROM Job j
			INNER JOIN IlrJobMetaData meta ON j.JobId = meta.JobId
			INNER JOIN Collection c ON c.CollectionId = j.CollectionId AND c.Name = @collectionName
			INNER JOIN @expectedReturners e ON e.UkPrn = j.Ukprn
		) AS Submissions
		WHERE Rank = 1

	SELECT
		j.Ukprn,
		meta.FileName,
		meta.StorageReference
	FROM Job j
	INNER JOIN FileUploadJobMetaData meta ON j.JobId = meta.JobId
	INNER JOIN Collection c ON c.CollectionId = j.CollectionId
	INNER JOIN ReturnPeriod rp ON rp.CollectionId = j.CollectionId AND rp.PeriodNumber = meta.PeriodNumber
	INNER JOIN @latestSubmissions ls ON ls.JobId = j.JobId
	WHERE j.Status = @completeStatus
	AND c.Name = @collectionName
	AND rp.PeriodNumber < @periodNumber 
	AND j.Ukprn NOT IN (SELECT UkPrn FROM @currentPeriodSubmissions)
END