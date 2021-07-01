--	 ,=\.-----""""^==--
--	;;'( ,___, ,/~\;                  
--	'  )/>/   \|-,                  
--	   | `\    | "                  
--	   "   "   "  
CREATE PROCEDURE [dbo].[GetJobByPriority]
	@ResultCount int
AS
BEGIN
   SET NOCOUNT ON

	DECLARE @ConcurrentExecutionCount int = 0
	DECLARE @NumJobsRunning int = 0

	DECLARE @CollectionTypeId int;
	DECLARE @CollectionId int;

	-- The table structure returned to the caller
	DECLARE @ReturnResults Table (
		[JobId] bigint,
		CollectionId int,
		[Priority] smallint,
		[DateTimeCreatedUTC] datetime,
		[DateTimeSubmittedUtc] datetime NULL,
		[DateTimeUpdatedUTC] datetime,
		[Status] smallint,
		[RowVersion] binary(8),
		[CreatedBy] varchar(50),
		[NotifyEmail] nvarchar(500),
		[CrossLoadingStatus] smallint,
		[Ukprn] bigint,
		[CollectionTypeId] int,
		[RowNumber] int
	)

	-- Get all currently running Ukprns so that we don't try and create them as jobs
	DECLARE @RunningUkprns Table (
		[Ukprn] bigint,
		[CollectionId] int,
		[CollectionTypeId] int
	)

	SELECT @CollectionTypeId = [CollectionTypeId]
	FROM [dbo].[CollectionType]
	WHERE [Type] = 'ILR'

	-- Check for any running reference data jobs that prevent us from running normal jobs		
	SELECT @NumJobsRunning = Count(JobId)
	FROM [dbo].[Job] j WITH (nolock)
	INNER JOIN [dbo].[Collection] c WITH (NOLOCK)
		ON [c].[CollectionId] = [j].[CollectionId]
	INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK)
		ON [c].[CollectionTypeId] = [ct].[CollectionTypeId]
	WHERE [j].[Status] IN (2,3)
		AND [ct].[Type] = 'REF'

	-- Check for waiting reference data jobs
	INSERT INTO @ReturnResults ([JobId], [CollectionId], [Priority], [DateTimeCreatedUTC], [DateTimeSubmittedUtc], [DateTimeUpdatedUTC], [Status], [RowVersion], [CreatedBy], [NotifyEmail], [CrossLoadingStatus], [Ukprn], [CollectionTypeId], [RowNumber])
	SELECT [j].[JobId]
		  ,[j].[CollectionId]
		  ,[j].[Priority]
		  ,[j].[DateTimeCreatedUTC]
		  ,NULL [DateTimeSubmittedUtc]
		  ,[j].[DateTimeUpdatedUTC]
		  ,[j].[Status]
		  ,[j].[RowVersion]
		  ,[j].[CreatedBy]
		  ,[j].[NotifyEmail]
		  ,[j].[CrossLoadingStatus]
		  ,[j].[Ukprn]
		  ,[c].[CollectionTypeId]
		  ,1
	FROM [dbo].[Job] j WITH (nolock)
	INNER JOIN [dbo].[Collection] c WITH (NOLOCK)
		ON [c].[CollectionId] = [j].[CollectionId]
	INNER JOIN [dbo].[CollectionType] ct WITH (NOLOCK)
		ON [c].[CollectionTypeId] = [ct].[CollectionTypeId]
	INNER JOIN JobTopicSubscription jts
		ON [jts].[CollectionId] = [j].[CollectionId] 
	LEFT JOIN JobSubscriptionTask jst
		ON [jst].JobTopicId = jts.JobTopicId
	WHERE [j].[Status] = 1
		AND [jts].[TopicOrder] = 1
		AND [jts].[Enabled] = 1
		AND [jst].[Enabled] = 1
		AND [jst].[TaskOrder] = 1
		AND [c].[ProcessingOverrideFlag] !=0
		AND [ct].[Type] = 'REF'

	IF @@ROWCOUNT = 0 AND @NumJobsRunning = 0
	BEGIN
		-- Retrieve running jobs (ignore ILR validation jobs)
		INSERT INTO @RunningUkprns ([Ukprn], [CollectionId], [CollectionTypeId])
		SELECT ISNULL([j].[Ukprn], 0) [Ukprn], [j].[CollectionId], [c].[CollectionTypeId]
		FROM [dbo].[Job] j WITH (NOLOCK)
		INNER JOIN [dbo].[Collection] c
			ON [c].[CollectionId] = [j].[CollectionId]
		LEFT JOIN (Select JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc from [dbo].[IlrJobMetaData] WITH (NOLOCK) Group by jobid) ijmd on ijmd.JobId = j.JobId
		WHERE [j].[Status] IN (2, 3) AND (([c].[CollectionTypeId] = @CollectionTypeId AND ijmd.DateTimeSubmittedUtc IS NOT NULL) OR [c].[CollectionTypeId] <> @CollectionTypeId)

		-- If not reference data jobs, then look for normal jobs. Use row_number as protection from running the same queued Ukprn 2+ times.
		INSERT INTO @ReturnResults ([JobId], CollectionId, [Priority], [DateTimeCreatedUTC], [DateTimeSubmittedUtc], [DateTimeUpdatedUTC], [Status], [RowVersion], [CreatedBy], [NotifyEmail], [CrossLoadingStatus], [Ukprn], [CollectionTypeId], [RowNumber])
		SELECT [j].[JobId]
				,[j].[CollectionId]
				,[j].[Priority]
				,[j].[DateTimeCreatedUTC]
				,[ijmd].[DateTimeSubmittedUtc]
				,[j].[DateTimeUpdatedUTC]					  
				,[j].[Status]
				,[j].[RowVersion]
				,[j].[CreatedBy]
				,[j].[NotifyEmail]
				,[j].[CrossLoadingStatus]
				,ISNULL([j].[Ukprn], 0) [Ukprn]
				,[c].[CollectionTypeId]
				,ROW_NUMBER() OVER(PARTITION BY [j].[Ukprn], Case When ijmd.DateTimeSubmittedUtc IS NULL Then 1 Else 0 END ORDER BY [Priority] DESC, [j].[JobId]) rn
		FROM [dbo].[Job] j WITH (NOLOCK)
		INNER JOIN [dbo].[JobTopicSubscription] jts
			ON [jts].[CollectionId] = [j].[CollectionId] 
		LEFT JOIN [dbo].[JobSubscriptionTask] jst
			ON [jst].[JobTopicId] = [jts].[JobTopicId]
		LEFT JOIN [dbo].[FileUploadJobMetaData] meta WITH (NOLOCK)
			ON [j].[JobId] = [meta].[JobId]
		LEFT JOIN [dbo].[NcsJobMetaData] ncsmeta WITH (NOLOCK)
			ON [j].[JobId] = [ncsmeta].[JobId]
		LEFT JOIN [dbo].[Collection] c ON [c].[CollectionId] = [j].[CollectionId]
		LEFT JOIN (Select JobId, Max(DateTimeSubmittedUtc) DateTimeSubmittedUtc from [dbo].[IlrJobMetaData] WITH (NOLOCK) Group by jobid) ijmd on ijmd.JobId = j.JobId
		WHERE [j].[Status] = 1 -- Job is ready
			AND [jts].[TopicOrder] = 1 -- Only want first row from linked topics
			AND [jts].[Enabled] = 1 -- (first) Topic is enabled
			AND [jst].TaskOrder = 1
			AND [jst].[Enabled] = 1
			AND IsNull([jts].IsFirstStage, 0) = Case When ijmd.DateTimeSubmittedUtc IS NULL and c.MultiStageProcessing = 1 Then 1 Else 0 END --Only pick topic for stage we're at
			AND ISNULL([c].[IsOpen], 1) = 1 -- Collection is open
			AND dbo.CanProcessJob(c.CollectionId, 
								 COALESCE(meta.PeriodNumber, ncsmeta.PeriodNumber), 
								 Case When ijmd.DateTimeSubmittedUtc Is Null Then 1 Else 0 END, 
								 c.ProcessingOverrideFlag, 
								 c.CollectionTypeId, 
								 c.MultiStageProcessing,
								 j.[Priority],
								 COALESCE(j.DateTimeUpdatedUTC, j.DateTimeCreatedUTC)) = 1
		ORDER BY [j].[Priority] DESC, [j].[JobId]

		-- Delete duplicate UKRPNs and already running where not system jobs
		DELETE FROM @ReturnResults
		WHERE [Ukprn] <> 0
			AND (
				([RowNumber] > 1 AND ([CollectionTypeId] <> @CollectionTypeId OR ([CollectionTypeId] = @CollectionTypeId AND [DateTimeSubmittedUtc] IS NOT NULL)))
				OR ([CollectionTypeId] = @CollectionTypeId AND [DateTimeSubmittedUtc] IS NOT NULL AND [Ukprn] IN (Select Ukprn FROM @RunningUkprns))
				)
	END

    -- Delete the jobs we don't have capacity for (as controlled by the Collection Type configuration)
    ;WITH RANKED_JOBS AS
    (
        SELECT JobId, ROW_NUMBER() OVER (PARTITION BY [CollectionTypeId] ORDER BY [Priority] DESC, [JobId]) AS RowNumber
        FROM @ReturnResults
    )

    DELETE ru
    FROM @ReturnResults ru
    INNER JOIN RANKED_JOBS rj on rj.JobId = ru.JobId
    INNER JOIN [dbo].[CollectionType] ct on ct.CollectionTypeId = ru.CollectionTypeId
    LEFT JOIN (Select [CollectionTypeId], COUNT([Ukprn]) cnt from @RunningUkprns GROUP BY [CollectionTypeId]) ruc on ruc.CollectionTypeId = ru.CollectionTypeId
    WHERE rj.RowNumber > (ct.ConcurrentExecutionCount - COALESCE(ruc.cnt, 0)) and (ru.CollectionTypeId <> 1 or (ru.CollectionTypeId = 1 and DateTimeUpdatedUTC is not null))

	-- Return the final list of jobs back
	SELECT Top (@ResultCount) [JobId], [CollectionId], [Priority], [DateTimeCreatedUTC], [DateTimeUpdatedUTC], [Status], [RowVersion], [CreatedBy], [NotifyEmail], [CrossLoadingStatus], [Ukprn]
	FROM @ReturnResults

END

GO

GRANT EXECUTE
    ON OBJECT::[dbo].[GetJobByPriority] TO [JobManagementSchedulerUser]
    AS [dbo];

GO
