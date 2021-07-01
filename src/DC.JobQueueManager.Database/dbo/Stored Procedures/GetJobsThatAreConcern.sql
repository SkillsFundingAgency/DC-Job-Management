CREATE PROCEDURE [dbo].[GetJobsThatAreConcern]
	@NOWUTC DATETIME
AS
BEGIN
	DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
	DECLARE @json VARCHAR(1000) = N'{"collectionTypes" : ["ILR","EAS","ESF","NCS"]}'

	INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
	EXEC [dbo].[GetOpenPeriods] @nowutc = @NOWUTC, @collectiontypes = @json, @includePeriodEnd = 0
	
	DECLARE @concerns TABLE (
		[JobId] bigint,
		[CollectionYear] int,
        [Ukprn] bigint,
        [FileName] varchar(100),
        [LastSuccessfulSubmission] datetime,
        [PeriodOfLastSuccessfulSubmission] int,
        [CollectionType] varchar(50)
    )

	DECLARE @periodNumber INT
    DECLARE @collectionYear INT
	DECLARE @collectionType VARCHAR(50)

	DECLARE Period_Cursor CURSOR FORWARD_ONLY FOR  
    SELECT [CollectionType], [Period], [Year]
    FROM @yearsAndPeriods

	OPEN Period_Cursor;  
    FETCH NEXT FROM Period_Cursor
    INTO @collectionType, @PeriodNumber, @CollectionYear;  
    WHILE @@FETCH_STATUS = 0  
       BEGIN
          Insert into @concerns
            Exec [dbo].[GetJobConcernsPerCollectionPerPeriod] @collectionType, @collectionYear, @periodNumber			
        FETCH NEXT FROM Period_Cursor
        INTO @collectionType, @PeriodNumber, @CollectionYear;
       END;  
    CLOSE Period_Cursor;  
    DEALLOCATE Period_Cursor;

SELECT * FROM @concerns

END
GO

