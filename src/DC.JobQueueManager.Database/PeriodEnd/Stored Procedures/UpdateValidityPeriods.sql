CREATE PROCEDURE [UpdateValidityPeriods]
	@collectionYear INT,
	@period INT,
	@validityPeriods VARCHAR(MAX)
AS
	
	DECLARE @newValidities TABLE ([Id] INT, [EntityType] INT, [Enabled] BIT)
	INSERT @newValidities
	SELECT * 
	FROM OPENJSON(@validityPeriods)
	WITH (
		[Id] INT '$.Id',
		[EntityType] INT '$.EntityType',
		[Enabled] BIT '$.Enabled'
	)

	UPDATE vp
	SET vp.[Enabled] = nv.[Enabled]
	FROM PeriodEnd.SubPathValidityPeriod vp
	INNER JOIN @newValidities nv ON vp.HubPathId = nv.Id 
		AND nv.EntityType = 1
	WHERE vp.CollectionYear = @collectionYear
	AND vp.[Period] = @period

	UPDATE vp
	SET vp.[Enabled] = nv.[Enabled]
	FROM PeriodEnd.ValidityPeriod vp
	INNER JOIN @newValidities nv ON vp.[HubPathItemId] = nv.[Id] 
		AND nv.[EntityType] = 2
	WHERE vp.[CollectionYear] = @collectionYear
	AND vp.[Period] = @period

	UPDATE vp
	SET vp.[Enabled] = nv.[Enabled]
	FROM PeriodEnd.EmailValidityPeriod vp
	INNER JOIN @newValidities nv ON vp.[HubPathItemId] = nv.Id 
		AND nv.[EntityType] = 3
	WHERE vp.[CollectionYear] = @collectionYear
	AND vp.[Period] = @period