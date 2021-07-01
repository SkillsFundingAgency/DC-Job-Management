CREATE PROCEDURE GetPeriodEndJobs
(
    @pathId INT,
	@collectionYear INT,
	@periodNumber INT
)
AS
    SET NOCOUNT ON

-----Job Summaries------------------------------------------------------------------------------------------------------------
	DECLARE @LatestDasJobs TABLE (
        [MaxJobId] INT,
        [UkPrn] INT,
		[PathItemId] INT
    )

	-- Get Latest Das jobs for return period
	INSERT INTO @LatestDasJobs ([MaxJobId], [UkPrn], [PathItemId])
		SELECT MAX(j.JobId) MaxJobId, j.Ukprn, pij.PathItemId 
		FROM PeriodEnd.PathItemJob pij
		INNER JOIN [dbo].[Job] j ON j.JobId = pij.JobId
		INNER JOIN [PeriodEnd].[PathItem] pi ON pi.PathItemId = pij.PathItemId
		INNER JOIN [PeriodEnd].[Path] p ON p.PathId = pi.PathId
		INNER JOIN [PeriodEnd].[PeriodEnd] pe ON pe.PeriodEndId = p.PeriodEndId
		INNER JOIN [dbo].[ReturnPeriod] rp ON rp.ReturnPeriodId = pe.PeriodId
		INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
		WHERE c.ResubmitJob = 0
		AND rp.PeriodNumber = @periodNumber
		GROUP BY j.Ukprn, pij.PathItemId


	SELECT p.HubPathId, pi.Ordinal, pi.PathItemLabel,
		COUNT(CASE WHEN j.Status = 1 OR j.Status = 2 THEN 1 ELSE NULL END) NumberOfWaitingJobs,
		COUNT(case WHEN j.Status = 4 THEN 1 ELSE NULL END) NumberOfCompleteJobs,
		COUNT(case WHEN j.Status = 6 OR j.Status = 5 THEN 1 ELSE NULL END) NumberOfFailedJobs,
		COUNT(case WHEN j.Status = 3 THEN 1 ELSE null END) NumberOfRunningJobs
	FROM [dbo].[Collection] c
	INNER JOIN [dbo].[ReturnPeriod] rp ON rp.CollectionId = c.CollectionId
	INNER JOIN [PeriodEnd].[PeriodEnd] pe ON pe.PeriodId = rp.ReturnPeriodId
	INNER JOIN [PeriodEnd].[Path] p ON p.PeriodEndId = pe.PeriodEndId
	INNER JOIN [PeriodEnd].[PathItem] pi ON pi.PathId = p.PathId
	INNER JOIN [PeriodEnd].[PathItemJob] pij ON pij.PathItemId = pi.PathItemId
	INNER JOIN [dbo].[Job] (NOLOCK) j ON j.JobId = pij.JobId
	INNER JOIN [dbo].[Collection] jc ON jc.CollectionId = j.CollectionId
	LEFT JOIN @LatestDasJobs ldj ON ldj.UkPrn = j.Ukprn AND ldj.MaxJobId = pij.JobId AND pij.PathItemId = ldj.PathItemId
	WHERE (@collectionYear = 0 OR c.CollectionYear = @collectionYear) 
	AND rp.PeriodNumber = @periodNumber 
	AND (jc.ResubmitJob = 1 OR (jc.ResubmitJob = 0 AND ldj.UkPrn IS NOT NULL))
	AND (p.HubPathId = @pathId or @pathId = -1)
	GROUP BY p.HubPathId, pi.Ordinal, pi.PathItemLabel

------------------------------------------------------------------------------------------------------------------------------
	DECLARE @Data TABLE (
		HubPathId INT,
		JobId BIGINT,
		[Status] INT,
		[Rank] INT,
		Ordinal INT,
		ProviderName NVARCHAR(MAX),
		RowNumber INT
	)

	-- Return 10 jobs per pathitem unless more than 10 retries, then return all failed (only failed) for pathitem
	-- Job ordering (Failed Retry (5 with no newer job), processing (3), Waiting (1,2), Completed (4), Failed (5))
	-- Get all jobs ranked
	INSERT INTO @Data
	SELECT 
		HubPathId, 
		JobId,
		[Status],
		CASE [Status] 
			WHEN 5 THEN
				CASE WHEN ranked.rkn = 1 AND ranked.rk = 1 THEN -1 ELSE 4 END  -- If it's failed, then we may want to treat it as a high priority retry
			WHEN 4 THEN 3 
			WHEN 3 THEN 1 
			WHEN 2 THEN 2 
			WHEN 1 THEN 2 
		END [Rank], 
		Ordinal, 
		ProviderName,
		ROW_NUMBER() OVER (PARTITION BY HubPathId, Ordinal ORDER BY 
								CASE [Status] 
									WHEN 5 THEN
										CASE WHEN ranked.rkn = 1 AND ranked.rk = 1 THEN -1 ELSE 4 END  -- If it's failed, then we may want to treat it as a high priority retry
									WHEN 4 THEN 3 WHEN 3 THEN 1 WHEN 2 THEN 2 WHEN 1 THEN 2 
								END,
								JobId DESC
							) [RowNumber] -- Global row number for jobs in the path item taking into account the correct ordering mechanism
	FROM (
		SELECT 
			p.HubPathId,
			j.JobId, 
			pi.Ordinal, 
			[Status], 
			COALESCE(mca.GLACode, CAST(j.Ukprn AS NVARCHAR(MAX))) ProviderName,
			RANK() OVER (PARTITION BY HubPathId, pi.Ordinal, [Status], COALESCE(mca.GLACode, CAST(j.Ukprn AS NVARCHAR(MAX))) ORDER BY j.JobId DESC) rk, -- Rank the job in terms of same provider jobs with status
			ROW_NUMBER() OVER (PARTITION BY HubPathId, pi.Ordinal, COALESCE(mca.GLACode, CAST(j.Ukprn AS NVARCHAR(MAX))) ORDER BY j.JobId DESC) rkn -- Number the job in terms of same provider jobs
		FROM [dbo].[Collection] c
		INNER JOIN [dbo].[ReturnPeriod] rp ON rp.CollectionId = c.CollectionId
		INNER JOIN [PeriodEnd].[PeriodEnd] pe ON pe.PeriodId = rp.ReturnPeriodId
		INNER JOIN [PeriodEnd].[Path] p ON p.PeriodEndId = pe.PeriodEndId
		INNER JOIN [PeriodEnd].[PathItem] pi ON pi.PathId = p.PathId
		INNER JOIN [PeriodEnd].[PathItemJob] pij ON pij.PathItemId = pi.PathItemId
		INNER JOIN [dbo].[Job] (NOLOCK) j ON j.JobId = pij.JobId
		LEFT JOIN [dbo].[MCADetail] mca ON mca.Ukprn = j.Ukprn
		WHERE (@collectionYear = 0 OR c.CollectionYear = @collectionYear) 
		AND rp.PeriodNumber = @periodNumber 
		AND (p.HubPathId = @pathId OR @pathId = -1)
	) ranked

	;WITH grouped (Cnt, HubPathId, Ordinal)
	AS(
		SELECT COUNT(*) cnt, HubPathId, Ordinal 
		FROM @Data dd  
		WHERE [Rank] = -1 
		GROUP BY HubPathId, ordinal
	)
	SELECT d.*, g.cnt 
	FROM @Data d
	LEFT JOIN grouped g ON g.HubPathId = d.HubPathId AND g.Ordinal = d.Ordinal
	WHERE (COALESCE(g.cnt, 0) <= 10 AND [RowNumber] <= 10)
		OR (COALESCE(g.cnt, 0) > 10 AND [RowNumber] <= COALESCE(g.cnt, 0))
	ORDER BY [Rank], JobId DESC

RETURN 0