CREATE PROCEDURE [dbo].[BulkAddProviderCollections] 
	-- Add the parameters for the stored procedure here
	@setEndDate varchar(max),
	@removeEndDate varchar(max),
	@add varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Set matching end dates
	UPDATE  [dbo].[OrganisationCollection] 
	SET EndDateTimeUtc = SetEndDate.[EndDateTimeUtc]
	FROM [dbo].[OrganisationCollection] 
	INNER JOIN OPENJSON(@setEndDate)
	WITH (
		[OrganisationId] INT '$.OrganisationId',
		[CollectionId] INT '$.CollectionId',
		[EndDateTimeUtc] DATETIME '$.EndDateTimeUtc'
	) SetEndDate
	ON [OrganisationCollection] .[OrganisationId] = SetEndDate.[OrganisationId] AND [OrganisationCollection].[CollectionId] = SetEndDate.[CollectionId];

	-- Remove matching end dates
	UPDATE  [dbo].[OrganisationCollection] 
	SET EndDateTimeUtc = null
	FROM [dbo].[OrganisationCollection] 
	INNER JOIN OPENJSON(@removeEndDate)
	WITH (
		[OrganisationId] INT '$.OrganisationId',
		[CollectionId] INT '$.CollectionId'
	) RemoveEndDate
	ON [OrganisationCollection] .[OrganisationId] = RemoveEndDate.[OrganisationId] AND [OrganisationCollection].[CollectionId] = RemoveEndDate.[CollectionId];

	-- Add matching entries
	INSERT [dbo].[OrganisationCollection]
			([OrganisationId]
			,[CollectionId]
			,[StartDateTimeUtc]
			,[EndDateTimeUtc]
			,[Ukprn])
	SELECT *
		FROM OPENJSON(@add)
		WITH (
			[OrganisationId] INT '$.OrganisationId',
			[CollectionId] INT '$.CollectionId',
			[StartDateTimeUtc] DATETIME '$.StartDateTimeUtc',
			[EndDateTimeUtc] DATETIME '$.EndDateTimeUtc',
			[Ukprn] BIGINT '$.Ukprn'
		); 
END;