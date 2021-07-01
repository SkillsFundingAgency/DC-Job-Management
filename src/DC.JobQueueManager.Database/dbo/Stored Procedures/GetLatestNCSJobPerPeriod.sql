-- Author:    Name
-- Create date:
-- Description:
-- =============================================
CREATE PROCEDURE [dbo].[GetLatestNCSJobPerPeriod]
  -- Add the parameters for the stored procedure here
  @ukprn              BIGINT,
  @currentDateTimeUtc DATETIME
AS
  BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SELECT *
      FROM   (SELECT j.JobId,
                     j.DateTimeCreatedUTC,
                     j.CollectionId,
                     j.Ukprn,
                     j.Status,
                     c.CollectionYear,
                     ct.Type as CollectionType,
                     ncs.ExternalTimestamp,
                     ncs.TouchpointId,
                     ncs.PeriodNumber,
                     Row_number()
                       OVER(
                         PARTITION BY c.CollectionId, ncs.PeriodNumber,
                       ncs.TouchpointId
                         ORDER BY j.DateTimeCreatedUTC DESC) AS [rn]
              FROM   Job j
                     INNER JOIN NcsJobMetaData ncs
                             ON ncs.JobId = j.JobId
                     INNER JOIN ReturnPeriod rp
                             ON rp.CollectionId = j.CollectionId
                                AND rp.PeriodNumber = ncs.PeriodNumber
                     INNER JOIN Collection c
                             ON c.CollectionId = j.CollectionId
                     INNER JOIN CollectionType ct
                             ON ct.CollectionTypeId = c.CollectionTypeId
              WHERE  j.Ukprn = @ukprn
                     AND j.Status = 4
                     AND ct.Type = 'NCS'
                     AND @currentDateTimeUtc > j.DateTimeCreatedUTC
					 )
					 x
      WHERE  x.rn = 1
  END