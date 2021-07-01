CREATE PROCEDURE [dbo].[GetLatestSubmittedJobs] 
	@collectionName VARCHAR(50) -- = 'ILR1819',
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @completeStatus INT = 4

	SELECT * FROM
		(
			SELECT
				j.Ukprn,
				j.FileName,
				j.StorageReference,
				ROW_NUMBER() OVER(PARTITION BY j.Ukprn,j.CollectionId ORDER BY j.DateTimeSubmittedUTC DESC) AS [rn]

			FROM  dbo.ReadOnlyJob j
			WHERE j.Status = @completeStatus
			And j.CollectionName = @collectionName
			And Ukprn > 0
		) x
	Where x.rn = 1
	
END