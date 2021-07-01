
CREATE VIEW [dbo].[vw_OrganisationCollectionAssignment]
AS 
SELECT TOP 100 PERCENT
	 C.[CollectionYear]	
	,C.[Name] as [Collection]
    ,OC.[Ukprn]
	,ISNULL(CONVERT(VARCHAR(25),OC.[StartDateTimeUtc],113),'') as [StartDateTimeUtc]
    ,ISNULL(CONVERT(VARCHAR(25),OC.[EndDateTimeUtc],113),'') as [EndDateTimeUtc]
	,O.[IsMCA]
	,C.[IsOpen] as CollectionIsOpen
	,C.[Description] CollectionType
	,CT.[Type]
FROM Organisation O
INNER JOIN [OrganisationCollection] OC on OC.[OrganisationId] = O.[OrganisationId]
INNER JOIN [Collection] C on OC.[CollectionId] = C.[CollectionId]
INNER JOIN [CollectionType] CT ON CT.[CollectionTypeId] = C.[CollectionTypeId]
ORDER BY OC.[Ukprn], C.[CollectionYear], C.[Name]
