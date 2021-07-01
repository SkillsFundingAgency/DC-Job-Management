CREATE PROCEDURE GetOpenPeriods 
	@nowutc DATETIME,
	@collectiontypes nvarchar(max),
	@includePeriodEnd bit
AS
	BEGIN
	-- @collectiontypes is a JSON array of collectionTypes to include and should take the form of for example .. N'{"collectionTypes" : ["ILR","EAS","NCS"]}'

		DECLARE @nowUtcToday DATETIME = DATEADD(DAY, DATEDIFF(DAY, 0, @nowutc), 0);
		DECLARE @YEARSANDPERIODS TABLE ([COLLECTIONTYPE] VARCHAR(50), [YEAR] INT, [PERIOD] INT);
		
		INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
		SELECT [TYPE], CollectionYear, PeriodNumber
		FROM (SELECT CT.Type AS [TYPE], [StartDateTimeUTC], LEAD(StartDateTimeUTC, 1, EndDateTimeUTC + 6) OVER (PARTITION BY RP.CollectionId ORDER BY [PeriodNumber]) [ENDDATETIMELEADUTC], [PeriodNumber], C.CollectionYear
				FROM [dbo].[ReturnPeriod] RP
					  INNER JOIN [dbo].[Collection] C ON C.CollectionId = RP.CollectionId
					  INNER JOIN [dbo].[CollectionType] CT ON CT.CollectionTypeId = C.CollectionTypeId
				WHERE CT.Type IN (SELECT [type] FROM OPENJSON(@collectiontypes) with (collectionTypes NVARCHAR(MAX) 'strict $.collectionTypes' AS JSON) ct OUTER APPLY OPENJSON(ct.collectionTypes) WITH ([type] NVARCHAR(10) '$'))) ILRPERIODSSTRETCHED
		WHERE StartDateTimeUTC <= @nowutc
				AND ENDDATETIMELEADUTC >= @nowutc;

		IF @includePeriodEnd = 1
		BEGIN
			INSERT INTO @YEARSANDPERIODS ([COLLECTIONTYPE], [YEAR], [PERIOD])
			SELECT 
				CASE ct.Type 
					WHEN 'ILR' THEN 'PE-ILR'
					WHEN 'NCS' THEN 'PE-NCS'
				END,
				c.CollectionYear,
				rp.PeriodNumber
			FROM PeriodEnd.PeriodEnd pe
			INNER JOIN dbo.ReturnPeriod rp ON rp.ReturnPeriodId = pe.PeriodId
			INNER JOIN dbo.Collection c ON c.CollectionId = rp.CollectionId
			INNER JOIN dbo.CollectionType ct ON c.CollectionTypeId = ct.CollectionTypeId
			WHERE pe.Closed <> 1 AND c.CollectionYear IS NOT NULL
		END

		SELECT * FROM @YEARSANDPERIODS
		
	END
GO

