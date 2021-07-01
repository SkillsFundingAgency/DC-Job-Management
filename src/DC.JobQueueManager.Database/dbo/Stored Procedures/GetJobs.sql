
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetJobs] 
	-- Add the parameters for the stored procedure here
	@period int  = null,
	@ukprn bigint = NULL,
	@startDateTimeUtc datetime = null,
	@endDateTimeUtc datetime = null,
	@jobStatus smallint = NULL,
	@isSubmitted BIT = NULL,
	@isCollectionUploadType bit = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT
	j.*
FROM  ReadOnlyJob j

WHERE 
	(@isSubmitted is null OR j.IsSubmitted = @isSubmitted)
	AND (@ukprn is null OR j.Ukprn = @ukprn)
	AND (@period is null OR j.PeriodNumber = @period )
	AND (@jobStatus is null OR j.Status = @jobStatus)
	AND (@startDateTimeUtc is null OR j.DateTimeSubmittedUTC >= @startDateTimeUtc)
	AND (@endDateTimeUtc is null OR  j.DateTimeSubmittedUTC <= @endDateTimeUtc)
	AND (@isCollectionUploadType is null OR j.IsCollectionUploadType = @isCollectionUploadType)
END