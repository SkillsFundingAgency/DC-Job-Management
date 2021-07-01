CREATE VIEW [dbo].[vw_JobSchedules]
AS 
SELECT 
	 S.[CollectionId]
	,JT.Name
	,S.[Enabled]
	,S.[MinuteIsCadence]
	,S.[Minute]
	,S.[Hour]
	,S.[DayOfTheMonth]
	,S.[Month]
	,S.[DayOfTheWeek]
	,S.[ExecuteOnceOnly]
	,S.[LastExecuteDateTime]
FROM [dbo].[Schedule] S
INNER JOIN [dbo].[Collection] JT 
	ON JT.[CollectionId] = S.[CollectionId]

