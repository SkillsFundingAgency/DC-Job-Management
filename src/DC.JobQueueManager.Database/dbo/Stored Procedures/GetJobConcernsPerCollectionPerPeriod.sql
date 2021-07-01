CREATE PROCEDURE [dbo].[GetJobConcernsPerCollectionPerPeriod]
(
	@collectionType VARCHAR(100),
    @collectionYear INT,
	@periodNumber INT
)
AS
BEGIN

--Status Descriptions
--1	Ready
--2	MovedForProcessing
--3	Processing
--4	Completed
--5	FailedRetry
--6	Failed
--7	Paused
--8	Waiting
DECLARE @JobsProcessing TABLE (Ukprn BIGINT, JobId BIGINT, DateTimeSubmittedUtc DATETIME, Status SMALLINT, FileName VARCHAR(250), PeriodNumber INT, CollectionId INT,  CollectionType varchar(50), RowNumber BIGINT)

IF @collectionType = 'ILR'
BEGIN
	INSERT INTO @JobsProcessing
	SELECT 
		j.Ukprn, 
		j.JobId as JobId, 
		MAX(ilr.DateTimeSubmittedUtc),
		j.Status,
		fumd.FileName,
		fumd.PeriodNumber,
		c.CollectionId,
		CT.[Type] as CollectionType,
		RowNumber = ROW_NUMBER() OVER(PARTITION BY j.Ukprn ORDER BY ilr.DateTimeSubmittedUtc DESC)
	FROM 
		dbo.IlrJobMetaData ilr (NOLOCK)
	INNER JOIN [dbo].[FileUploadJobMetaData] fumd (NOLOCK) on ilr.JobId = fumd.JobId
	INNER JOIN [dbo].[Job] j (NOLOCK) ON ilr.JobId = j.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	WHERE
		c.CollectionYear = @collectionYear AND
		fumd.PeriodNumber = @periodNumber	
	GROUP BY j.JobId, j.Ukprn, j.Status, fumd.FileName, fumd.PeriodNumber, c.CollectionId, ilr.DateTimeSubmittedUTC, CT.[Type]
END

IF @collectionType = 'EAS'
BEGIN
	INSERT INTO @JobsProcessing
	SELECT
		j.Ukprn,
		j.JobId as JobId,
		NULL as DateTimeSubmittedUtc,
		Status,
		fumd.FileName,
		fumd.PeriodNumber,
		c.CollectionId,
		CT.[Type] as CollectionType,
		RowNumber = ROW_NUMBER() OVER(PARTITION BY j.Ukprn ORDER BY eas.JobId DESC)
	FROM [dbo].[EasJobMetaData] eas (NOLOCK)
	inner join [dbo].[FileUploadJobMetaData] fumd (NOLOCK) on eas.JobId = fumd.JobId
	inner join [dbo].[Job] j (NOLOCK) ON eas.JobId = j.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	where c.CollectionYear = @collectionYear AND
		fumd.PeriodNumber = @periodNumber
END

IF @collectionType = 'ESF'
BEGIN
	INSERT INTO @JobsProcessing
	SELECT
		j.Ukprn,
		j.JobId as JobId,
		NULL as DateTimeSubmittedUtc,
		Status,
		fumd.FileName,
		fumd.PeriodNumber,
		c.CollectionId,
		CT.[Type] as CollectionType,
		RowNumber = ROW_NUMBER() OVER(PARTITION BY j.Ukprn, esf.ContractReferenceNumber ORDER BY esf.JobId DESC)
	FROM [dbo].[EsfJobMetaData] esf (NOLOCK)
	inner join [dbo].[FileUploadJobMetaData] fumd (NOLOCK) on esf.JobId = fumd.JobId
	inner join [dbo].[Job] j (NOLOCK) ON esf.JobId = j.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	where c.CollectionYear = @CollectionYear AND
		fumd.PeriodNumber = @PeriodNumber
END

IF @collectionType = 'NCS'
BEGIN
	INSERT INTO @JobsProcessing
	SELECT 
		j.Ukprn,
		j.JobId as JobId,
		NULL as DateTimeSubmittedUtc,
		Status,
		ncs.TouchpointId FileName,
		ncs.PeriodNumber,
		c.CollectionId,
		CT.[Type] as CollectionType,
		RowNumber = ROW_NUMBER() OVER(PARTITION BY j.Ukprn, ncs.[TouchpointId] ORDER BY ncs.JobId DESC)
	FROM [dbo].[NcsJobMetaData] ncs (NOLOCK)
	inner join [dbo].[Job] j (NOLOCK) ON ncs.JobId = j.JobId
	INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
	INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
	where c.CollectionYear = @CollectionYear AND
		ncs.PeriodNumber = @PeriodNumber
END
	SELECT 
		jobsProcessing.JobId,
		@collectionYear,
		jobsProcessing.Ukprn,
		FileName, 
		latestSuccesful.latest LastSuccessfulSubmission, 
		latestSuccesful.PeriodNumber PeriodOfLastSuccessfulSubmission,
		CollectionType
	FROM
	@JobsProcessing jobsProcessing
	LEFT JOIN 
		(SELECT j.Ukprn, COALESCE(ncs.PeriodNumber, f.PeriodNumber) PeriodNumber, j.CollectionId, MAX(j.DateTimeUpdatedUTC) as latest,
			RowNumber = CASE 
							WHEN ct.Type = 'NCS' THEN ROW_NUMBER() OVER(PARTITION BY j.Ukprn, ncs.TouchpointId ORDER BY j.DateTimeUpdatedUTC DESC) 
							ELSE ROW_NUMBER() OVER(PARTITION BY j.Ukprn, j.CollectionId ORDER BY j.DateTimeUpdatedUTC DESC) 
						END
			FROM dbo.Job j
			INNER JOIN dbo.Collection C ON j.CollectionId = C.CollectionId
			INNER JOIN dbo.CollectionType ct ON C.CollectionTypeId = C.CollectionTypeId
			LEFT JOIN dbo.FileUploadJobMetaData f ON j.JobId = f.JobId
			LEFT JOIN dbo.NcsJobMetaData ncs ON j.JobId = ncs.JobId
			WHERE j.Status = 4 AND c.CollectionId IN (SELECT DISTINCT CollectionId FROM @JobsProcessing) 
			GROUP BY j.Ukprn, ct.Type, f.PeriodNumber, ncs.PeriodNumber, ncs.TouchpointId, j.CollectionId, j.DateTimeUpdatedUTC) as latestSuccesful
	ON jobsProcessing.Ukprn = latestSuccesful.Ukprn AND jobsProcessing.CollectionId = latestSuccesful.CollectionId
	WHERE jobsProcessing.RowNumber = 1 AND jobsProcessing.Status in (5,6) AND latestSuccesful.RowNumber = 1
END
