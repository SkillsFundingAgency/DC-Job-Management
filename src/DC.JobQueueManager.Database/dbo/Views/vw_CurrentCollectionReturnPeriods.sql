CREATE VIEW [dbo].[vw_CurrentCollectionReturnPeriods]
AS 
SELECT TOP 100 PERCENT
	   O.[Ukprn] AS UKPRN
	  ,C.[CollectionId] AS CollectionId
	  ,C.[Name] AS CollectionName
	  ,C.[IsOpen] AS IsOpen
	  ,CT.[Type] AS CollectionType
	  ,CT.[Description] AS Description
      ,RP.[StartDateTimeUTC] AS StartDateTimeUTC
      ,RP.[EndDateTimeUTC] AS EndDateTimeUTC
      ,RP.[PeriodNumber] AS PeriodNumber
      ,RP.[CalendarMonth] AS CalendarMonth
      ,RP.[CalendarYear] AS CalendarYear
	  ,ISNULL(OC.[StartDateTimeUtc],CONVERT(DATETIME,'2018-AUG-01')) AS OrgCollectionStartDateUTC
	  ,ISNULL(OC.[EndDateTimeUtc],CONVERT(DATETIME,'2600-DEC-31'))  AS OrgCollectionEndDateUTC
	  ,CONVERT(BIT,CASE WHEN GETUTCDATE() BETWEEN ISNULL(OC.[StartDateTimeUtc],CONVERT(DATETIME,'2018-AUG-01')) AND ISNULL(OC.[EndDateTimeUtc],CONVERT(DATETIME,'2600-DEC-31')) THEN 1 ELSE 0 END) AS IsOrgCollection_Enabled
FROM [dbo].[Organisation] O
INNER JOIN [dbo].[OrganisationCollection] OC
	ON OC.[OrganisationId] = O.[OrganisationId]
INNER JOIN [dbo].[Collection] C
	ON C.[CollectionId] = OC.[CollectionId]
INNER JOIN [dbo].[CollectionType] CT
	ON C.[CollectionTypeId] = CT.[CollectionTypeId]
INNER JOIN [dbo].[ReturnPeriod] RP
	ON RP.[CollectionId] = C.[CollectionId]
   AND GETUTCDATE() BETWEEN RP.[StartDateTimeUTC] AND RP.[EndDateTimeUTC]
ORDER BY UKPRN, [CalendarYear] DESC, [CalendarMonth] DESC , [PeriodNumber]




   