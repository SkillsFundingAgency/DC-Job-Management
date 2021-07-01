-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetLatestJobPerPeriod] 
	-- Add the parameters for the stored procedure here
	@ukprn bigint,
	@currentDateTimeUtc datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

SELECT * FROM (
SELECT
	j.*,
	ROW_NUMBER() OVER(PARTITION BY j.CalendarYear, j.PeriodNumber, j.CollectionName, j.ContractReferenceNumber ORDER BY j.DateTimeSubmittedUTC DESC) AS [rn]
FROM  ReadOnlyJob j 

LEFT JOIN ReturnPeriod rp 
	On rp.CollectionId = j.CollectionId
	And rp.PeriodNumber = j.PeriodNumber

WHERE 	j.Ukprn = @ukprn and j.Status = 4 AND IsCollectionUploadType = 1 and FileName <> '' And  @currentDateTimeUtc > rp.EndDateTimeUTC
) x
where x.rn = 1
	
END