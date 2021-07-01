CREATE PROCEDURE GetNCSLatestFailedJobsPerCollectionPerPeriod
(
    @collectionYear INT,
	@periodNumber INT
)
AS
BEGIN

		SELECT 	x.[JobId],
			x.[Ukprn],
			x.[CollectionName],
			x.[CollectionType],
			x.[FileName],
			X.[DateTimeSubmitted],
			x.[Status] 
	FROM 
	(
		SELECT  J.[JobId],
				J.[Ukprn],
				META.TouchpointId,
				C.[Name] As CollectionName,
				CT.[Type] As CollectionType,
				META.[ReportFileName] As FileName,
				J.[Status],
				J.[DateTimeCreatedUTC] AS DateTimeSubmitted,
				DENSE_RANK() OVER  (PARTITION BY J.[Ukprn], META.TouchpointId, J.[CollectionId], META.[PeriodNumber] ORDER BY  J.[DateTimeCreatedUTC] desc) AS SubmissionOrder
		FROM [dbo].[Job] AS J
		INNER JOIN [dbo].[Collection] AS C ON C.[CollectionId] = J.[CollectionId] 
		INNER JOIN [dbo].[CollectionType] AS CT ON CT.[CollectionTypeId] = C.[CollectionTypeId] 
		LEFT JOIN [dbo].[NcsJobMetaData] AS META ON META.[JobId] = J.[JobId]
		WHERE 
		C.[CollectionYear] = @collectionYear AND 
		META.[PeriodNumber] = @periodNumber AND
		CT.[Type] = 'NCS' 			
		
	) X 
	WHERE SubmissionOrder =1 
	AND Status in (5,6)

END