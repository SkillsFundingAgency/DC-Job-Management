CREATE PROCEDURE [dbo].[GetLatestDASSubmittedJobs] 
	@ilrCollectionName VARCHAR(50), -- = 'ILR2021',
	@dasSubmissionCollectionName VARCHAR(50) -- = 'PE-DAS-Submission2021'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @completeStatus INT = 4

	SELECT
		JobId,
		Ukprn
	FROM
		(
			SELECT
				j.JobId,
				j.Ukprn,
				ROW_NUMBER() OVER(PARTITION BY j.Ukprn ORDER BY COALESCE (meta.[DateTimeSubmittedUtc], j.DateTimeCreatedUTC)  DESC) AS [rn]

			FROM  dbo.Job j
			INNER JOIN [Collection] c ON j.CollectionId = c.CollectionId
			LEFT JOIN IlrJobMetaData meta ON j.JobId = meta.JobId
			WHERE j.[Status] = @completeStatus
			AND c.[Name] IN (@ilrCollectionName, @dasSubmissionCollectionName)
			AND Ukprn > 0
		) x
	WHERE x.rn = 1
	
END