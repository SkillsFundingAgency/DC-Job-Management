if(NOT EXISTS (SELECT * FROM [ReportsPublicationJobMetaData] WHERE ISNULL(ReportsPublished, 0) = 1))
BEGIN
  UPDATE p SET ReportsPublished = 1
  FROM [dbo].[ReportsPublicationJobMetaData] p
  JOIN JOB j ON j.jobId = p.JobId
  
  JOIN ReturnPeriod rp ON
  rp.PeriodNumber = p.PeriodNumber
  AND rp.CollectionId = j.CollectionId
  
  JOIN PeriodEnd.PeriodEnd pe
  ON pe.PeriodId = rp.ReturnPeriodId
  
  WHERE pe.FrmReportsPublished = 1
END