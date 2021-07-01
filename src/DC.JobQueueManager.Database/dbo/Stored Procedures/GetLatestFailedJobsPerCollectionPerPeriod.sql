CREATE PROCEDURE GetLatestFailedJobsPerCollectionPerPeriod
(
    @collectionYear INT,
	@periodNumber INT
)
AS
BEGIN

	DECLARE @Jobs TABLE 
		( [JobId] BIGINT PRIMARY KEY,
		  [UKPRN] BIGINT,
		  [CollectionId] INT,
		  [CollectionYear] INT,
		  [CollectionName] VARCHAR(50),
		  [CollectionType] VARCHAR(50),
		  [PeriodNumber] INT,
		  [Status] INT,
		  [IsCollectionUploadType] BIT,
		  [DateTimeCreatedUTC] DATETIME2,
		  [FileName] VARCHAR(250)
		)

	DECLARE @JobILRSubmittedDateTimeUTC TABLE 
		( [JobId] BIGINT PRIMARY KEY,
		  [SubmittedDateTimeUTC] DATETIME2
		)

	--Get Jobs we care about only
	INSERT INTO @Jobs ( [JobId], [UKPRN], [CollectionId], [PeriodNumber], [Status], [CollectionYear], [DateTimeCreatedUTC], [FileName], [CollectionName], [CollectionType])
	SELECT J.[JobId], J.[Ukprn], J.[CollectionId], META.[PeriodNumber], J.[Status], 
		   C.[CollectionYear], J.[DateTimeCreatedUTC], META.[FileName], 
		   C.[Name] AS [CollectionName], CT.[Type] AS [CollectionType]
	FROM [dbo].[Job] AS J
	INNER JOIN [dbo].[Collection] AS C ON C.[CollectionId] = J.[CollectionId] 
	INNER JOIN [dbo].[CollectionType] AS CT ON CT.[CollectionTypeId] = C.[CollectionTypeId] 
	LEFT JOIN [dbo].[FileUploadJobMetaData] AS META ON META.[JobId] = J.[JobId]
	WHERE C.[CollectionYear] = @collectionYear
		AND META.[PeriodNumber] = @periodNumber
		AND CT.[Type]  in ('ILR','ESF','EAS')
		
	
	-- Get Job Submitted Time
	INSERT INTO @JobILRSubmittedDateTimeUTC ([JobId], [SubmittedDateTimeUTC])
	SELECT M.[JobId], MAX(COALESCE (M.[DateTimeSubmittedUtc], J.[DateTimeCreatedUTC])) AS SubmittedDateTimeUTC
	FROM @Jobs AS J
	INNER JOIN [dbo].[IlrJobMetaData] AS M on J.[JobId] = M.[JobId]
	GROUP BY M.[JobId]

	-- Return Data
	SELECT 	x.[JobId],
			x.[UKPRN],
			x.[CollectionName],
			x.[CollectionType],
			x.[FileName],
			X.[DateTimeSubmitted],
			x.[Status] 
	FROM 
	(
		SELECT  J.[JobId],
				J.[UKPRN],
				J.[CollectionName],
				J.[CollectionType],
				J.[FileName],
				J.[Status],
				COALESCE ([SubmittedDateTimeUTC], [DateTimeCreatedUTC]) AS DateTimeSubmitted,
				DENSE_RANK() OVER  (PARTITION BY J.[UKPRN], J.[CollectionId], J.[PeriodNumber] ORDER BY COALESCE (s.[SubmittedDateTimeUTC], J.[DateTimeCreatedUTC]) DESC) AS SubmissionOrder
		FROM @Jobs AS J 
		LEFT JOIN @JobILRSubmittedDateTimeUTC AS S ON S.[JobId] = J.[JobId]
		WHERE j.CollectionType <> 'ILR' OR s.JobId IS NOT NULL
	) X 
	WHERE SubmissionOrder =1 
	AND Status in (5,6)
END