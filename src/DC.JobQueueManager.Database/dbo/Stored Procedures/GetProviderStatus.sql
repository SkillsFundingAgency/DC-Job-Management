CREATE PROCEDURE [dbo].[GetProviderStatus] 
	-- Add the parameters for the stored procedure here
	@ukprns varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH 
	ActiveAssignments AS 
	(SELECT DISTINCT OrganisationId FROM [dbo].[OrganisationCollection] WHERE StartDateTimeUtc <= GETUTCDATE() AND (EndDateTimeUtc IS NULL OR EndDateTimeUtc > GETUTCDATE())),

	InActiveAssignments AS 
	(SELECT DISTINCT OrganisationId FROM [dbo].[OrganisationCollection] WHERE StartDateTimeUtc >= GETUTCDATE() OR EndDateTimeUtc <= GETUTCDATE())

	SELECT Ukprns.ukprn, coalesce(o.IsMCA, 0) AS IsMCA,
	CASE 
		WHEN aa.OrganisationId IS NOT NULL THEN 1 -- 'ACTIVE' 
		WHEN aa.OrganisationId IS NULL AND ia.OrganisationId IS NOT NULL THEN 2 -- 'INACTIVE'
		WHEN aa.OrganisationId IS NULL AND ia.OrganisationId IS NULL AND o.OrganisationId IS NOT NULL THEN 3 --'NO ASSIGNMENT'
		WHEN aa.OrganisationId IS NULL AND ia.OrganisationId IS NULL AND o.OrganisationId IS NULL AND Ukprns.IsActive = 1 THEN 4 --'AVAILABLE'
		WHEN aa.OrganisationId IS NULL AND ia.OrganisationId IS NULL AND o.OrganisationId IS NULL AND Ukprns.IsActive = 0 THEN 5 --'DONT DISPLAY'
	END AS ProviderStatus

	FROM OPENJSON(@ukprns)
		WITH (
			[Ukprn] INT '$.Ukprn',
			[IsActive] BIT '$.IsActive'
		) Ukprns

	LEFT JOIN Organisation o ON o.Ukprn = Ukprns.ukprn
	LEFT JOIN ActiveAssignments AS aa ON aa.OrganisationId = o.OrganisationId
	LEFT JOIN InActiveAssignments AS ia ON ia.OrganisationId = o.OrganisationId;
 
END;