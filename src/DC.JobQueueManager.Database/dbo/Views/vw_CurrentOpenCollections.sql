CREATE VIEW [dbo].[vw_CurrentOpenCollections]
AS 
SELECT TOP 100 PERCENT
       CT.[Type] AS CollectionType
      ,C.[Name] AS CollectionName
      ,RP.[PeriodNumber] AS PeriodNumber
      ,C.[IsOpen] AS IsOpen
       ,GETUTCDATE() AS SYSTEM_DATETIME_NOW
      ,RP.[StartDateTimeUTC] AS StartDateTimeUTC
      ,RP.[EndDateTimeUTC] AS EndDateTimeUTC
      ,RP.[CalendarMonth] AS CalendarMonth
      ,RP.[CalendarYear] AS CalendarYear
      ,C.[CollectionId] AS CollectionId
FROM [dbo].[Collection] C
INNER JOIN [dbo].[CollectionType] CT
    ON C.[CollectionTypeId] = CT.[CollectionTypeId]
INNER JOIN [dbo].[ReturnPeriod] RP
    ON RP.[CollectionId] = C.[CollectionId]
   AND GETUTCDATE() BETWEEN RP.[StartDateTimeUTC] AND RP.[EndDateTimeUTC]
ORDER BY [CollectionType], [CalendarYear] DESC, [CalendarMonth] DESC , [PeriodNumber]