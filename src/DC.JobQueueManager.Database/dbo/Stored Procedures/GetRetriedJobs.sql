
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetRetriedJobs]
	-- Add the parameters for the stored procedure here
	@collectionName varchar(max),  
	@period int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select x.Id, x.JobId, x.DateTimeUTC from 
(SELECT
		Id,	 
	   [JobId]
      ,DateTimeSubmittedUtc as DateTimeUTC,
	  ROW_NUMBER() Over(partition by JobId order by Id) RowNumber
  FROM [dbo].[IlrJobMetaData]
) x
Where x.RowNumber >1
And x.JobId in (Select j.JobId from FileUploadJobMetaData meta inner join ReadOnlyJob j on j.JobId = meta.JobId
			     Where j.PeriodNumber = @period And CollectionName=@collectionName)	
END