
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetLatestJobs] 
	-- Add the parameters for the stored procedure here
	@ukprns varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Select 

	x.Ukprn,
	x.[CollectionId] ,
	x.CollectionName, 
	x.CollectionYear,
	x.PeriodNumber,
	x.JobId,
    x.DatetimesubmittedUTC as LastSubmittedDateUtc,
	x.[CreatedBy] as LastSubmittedBy
FROM	
	(SELECT
		j.Ukprn,
		j.[CollectionId] ,
		j.CollectionName, 
		j.CollectionYear,
		j.PeriodNumber,
		j.JobId,
		j.DateTimeSubmittedUTC,
		j.[CreatedBy],
		ROW_NUMBER() OVER(PARTITION BY j.Ukprn,j.CollectionId ORDER BY j.DateTimeSubmittedUTC DESC) AS [rn]
 
	FROM  ReadOnlyJob j
	INNER JOIN OPENJSON(@ukprns) with (ukprn bigint 'strict $') ukprns
		on j.Ukprn = Ukprns.ukprn
	WHERE j.Status = 4 AND IsCollectionUploadType = 1 
	) x

Where x.rn = 1	
	
END