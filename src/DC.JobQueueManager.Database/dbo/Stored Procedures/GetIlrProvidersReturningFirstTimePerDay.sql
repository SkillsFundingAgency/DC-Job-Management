CREATE PROCEDURE [dbo].[GetIlrProvidersReturningFirstTimePerDay]
(
    @collectionYear INT,
	@periodNumber INT
)
AS
BEGIN

--Status Descriptions
--4	Completed


DECLARE @CollectionId INT;
DECLARE @EndDateOfCollection DATE

SELECT 
    @CollectionId = CollectionId 
FROM Collection 
WHERE 
    CollectionTypeId = 1 AND 
    CollectionYear = @collectionYear


SELECT @EndDateOfCollection = CAST(rp.EndDateTimeUTC as DATE)
FROM
    [dbo].[ReturnPeriod] rp
WHERE 
    rp.CollectionId = @CollectionId AND 
    rp.PeriodNumber = @periodNumber

SELECT 
    COUNT(j.Ukprn) NumberOfSubmissions, 
    CAST(j.DateTimeCreatedUTC AS DATE) DateTimeCreatedUTC,
    DateDiff(DAY,  @EndDateOfCollection, CAST(j.DateTimeCreatedUTC AS DATE)) DaysToClose
FROM 
    [dbo].[Job] j
 INNER JOIN [dbo].[FileUploadJobMetaData] f ON f.JobId = j.JobId
 INNER JOIN (
        SELECT 
            MIN(j.DateTimeCreatedUTC) dt, j.Ukprn 
        FROM 
            [dbo].[Job] j
        INNER JOIN [dbo].[FileUploadJobMetaData] f ON f.JobId = j.JobId
        WHERE 
            f.PeriodNumber = @periodNumber AND 
            Status = 4 AND
            j.CollectionId = @CollectionId
        GROUP BY j.Ukprn) jf ON jf.dt = j.DateTimeCreatedUTC AND jf.Ukprn = j.Ukprn
      WHERE f.PeriodNumber = @periodNumber
      GROUP BY CAST(j.DateTimeCreatedUTC AS DATE)
 ORDER BY cast(j.DateTimeCreatedUTC AS DATE) ASC

END
GO
GRANT EXECUTE ON [dbo].[GetIlrProvidersReturningFirstTimePerDay] TO [DataViewer];
GO
