-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetUnSubmittedIlrJobs] 
	@ukprn bigint = NULL
AS
BEGIN
SET NOCOUNT ON;
	Declare @ConstILRCollectionType varchar(5) = 'ILR'

	Declare @ReturnResults Table (CollectionYear int,PeriodNumber int)

	Insert into @ReturnResults (CollectionYear, PeriodNumber)
	SELECT  CollectionYear, PeriodNumber FROM (
		Select CollectionYear, PeriodNumber,
            ROW_NUMBER() OVER(PARTITION BY CollectionYear ORDER BY PeriodNumber DESC ) AS [rn]
        From (
                Select DISTINCT c.CollectionYear, PeriodNumber
                From Job j inner join
                FileUploadJobMetaData meta on meta.JobId = j.JobId
                Join [Collection] c on c.CollectionId = j.CollectionId
                Join CollectionType ct on ct.CollectionTypeId = c.CollectionTypeId
                Where ct.Type = @ConstILRCollectionType And (@ukprn IS NULL OR j.Ukprn = @ukprn)
                )y
        ) x
        where x.rn in (1,2)


	SELECT
		j.*
	FROM  ReadOnlyJob j
	Inner Join @ReturnResults r
	ON r.CollectionYear = j.CollectionYear And r.PeriodNumber = j.PeriodNumber
	WHERE 
		j.IsSubmitted = 0 AND
		(@ukprn IS NULL OR j.Ukprn = @ukprn)
		AND j.CollectionType = @ConstILRCollectionType 
		ORDER BY j.CollectionYear DESC, j.PeriodNumber DESC
END