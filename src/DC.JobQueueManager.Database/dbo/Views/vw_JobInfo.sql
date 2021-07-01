CREATE VIEW [dbo].[vw_JobInfo]
AS 

SELECT ROJ.[JobId]
      ,ROJ.[CollectionId]
      ,ROJ.[Ukprn]
      ,ROJ.[DateTimeSubmittedUTC]
      ,ROJ.[Status]
      ,S.StatusDescription as JobStatus
      ,ROJ.[CreatedBy]
      ,ROJ.[PeriodNumber]
      ,ROJ.[CollectionName]
      ,ROJ.[CollectionYear]
      ,ROJ.[FileName]
      ,ROJ.[NotifyEmail]
      ,[CalendarMonth]
      ,[CalendarYear]
      ,[ReturnPeriodId]
      ,[CollectionType]
      ,[IsSubmitted]
      ,ROJ.[StorageReference]
      ,ROJ.[FileSize]
      ,[ExternalJobId]
      ,[TouchpointId]
      ,[ExternalTimestamp]
      ,[ReportFileName]
      ,ROJ.[DateTimeCreatedUTC]
	  ,ROJ.CollectionName As JobType
	  ,J.Priority
	  ,J.CrossLoadingStatus
	  , XLS.StatusDescription as CrossLoadingStatusDescription
	  ,CONVERT(varchar,[DateTimeSubmittedUTC],20) as 'Date Time Submitted'
      ,CONVERT(varchar,j.[DateTimeUpdatedUTC],20) as 'Date Time Updated'
  FROM [dbo].[ReadOnlyJob] (nolock) ROJ
  JOIN Job (nolock) J
  on J.JobId = ROJ.JobId
  Inner join [FileUploadJobMetaData] ful on ful.JobId = J.JobId
  Join JobStatusType S on S.StatusId = J.Status
  Left Join JobStatusType XLS on XLS.StatusId = CrossLoadingStatus
