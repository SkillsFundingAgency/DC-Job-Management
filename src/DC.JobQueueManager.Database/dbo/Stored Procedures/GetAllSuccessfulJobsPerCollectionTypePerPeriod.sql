CREATE PROCEDURE [dbo].[GetAllSuccessfulJobsPerCollectionTypePerPeriod]
(
	@collectionYear INT,
	@periodNumber INT,
	@collectionType VARCHAR(10)
)
AS
BEGIN
	SELECT 
		MAX(J.[JobId]) as JobId, 
		J.[Ukprn]
	FROM [dbo].[Job] AS J (NOLOCK)
	INNER JOIN [dbo].[Collection] (NOLOCK) AS C ON C.[CollectionId] = J.[CollectionId] 
	INNER JOIN [dbo].[CollectionType] (NOLOCK) AS CT ON CT.[CollectionTypeId] = C.[CollectionTypeId] 
	LEFT JOIN [dbo].[FileUploadJobMetaData] (NOLOCK) AS META ON META.[JobId] = J.[JobId]
	WHERE C.[CollectionYear] = @collectionYear
		AND META.[PeriodNumber] = @periodNumber
		AND CT.[Type] = @collectionType
		AND j.Status = 4
	GROUP BY j.Ukprn
END
GO