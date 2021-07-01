CREATE PROCEDURE [dbo].[GetFailedFilesCurrentPeriod]
AS
    DECLARE @nowUtc DATETIME = GETUTCDATE()

    DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 1

	SELECT 
		j.JobId as JobId,
		c.CollectionYear as CollectionYear,
		ct.Type as CollectionType,
		j.Ukprn as Ukprn,
		fujm.FileName as FileName,
		j.DateTimeUpdatedUTC as DateTimeOfFailure,
		j.[DateTimeUpdatedUTC] - j.DateTimeCreatedUTC as ProcessingTimeBeforeFailure
     FROM [dbo].[Job] j (NOLOCK)
		INNER JOIN [dbo].[Collection] c ON c.CollectionId = j.CollectionId
		INNER JOIN [dbo].[CollectionType] ct ON ct.CollectionTypeId = c.CollectionTypeId
		INNER JOIN [dbo].[FileUploadJobMetaData] (NOLOCK) fujm ON fujm.JobId = j.JobId
		INNER JOIN @YEARSANDPERIODS yp ON yp.COLLECTIONTYPE = ct.Type AND yp.[PERIOD] = fujm.[PeriodNumber] AND yp.[YEAR] = c.[CollectionYear]
	WHERE 
		j.Status in (5,6)
	
RETURN 0