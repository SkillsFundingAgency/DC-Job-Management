CREATE VIEW ReadOnlyJob
AS
SELECT   j.JobId, 
		j.CollectionId, 
		CASE WHEN j.Ukprn IS NULL THEN 0 ELSE j.Ukprn END As Ukprn, 
		COALESCE (ilr.SubmittedDateTimeUTC, j.DateTimeCreatedUTC) AS DateTimeSubmittedUTC, 
		j.Status, 
		j.CreatedBy, 
		COALESCE (rpd.PeriodNumber, rp.PeriodNumber, meta.PeriodNumber, ncs.PeriodNumber, pub.PeriodNumber, 0) AS PeriodNumber, 
		c.Name AS CollectionName, 
		CASE WHEN c.CollectionYear IS NULL THEN 0 ELSE c.CollectionYear END As CollectionYear,
		COALESCE (meta.FileName,'') As FileName, 
		j.NotifyEmail, 

		CASE WHEN rp.CalendarMonth IS NULL THEN 0 ELSE rp.CalendarMonth END As CalendarMonth,
		CASE WHEN rp.CalendarYear IS NULL THEN 0 ELSE rp.CalendarYear END As CalendarYear,
		CASE WHEN rp.ReturnPeriodId IS NULL THEN 0 ELSE rp.ReturnPeriodId END As ReturnPeriodId,
		
		ct.[Type] as CollectionType,
	    Cast(Case when ilr.SubmittedDateTimeUTC IS NOT NULL OR COALESCE(c.MultiStageProcessing,0) = 0 Then 1 Else 0 End As bit) As IsSubmitted,
		COALESCE(meta.StorageReference,pub.StorageReference,c.StorageReference,'') As StorageReference,
		CASE WHEN meta.FileSize IS NULL THEN 0 ELSE meta.FileSize END As FileSize,	
		COALESCE(ncs.ExternalJobId,'') as ExternalJobId,
		COALESCE(ncs.TouchpointId,'') as TouchpointId,
		COALESCE(ncs.ExternalTimestamp,'1900-01-01') as ExternalTimestamp,
		COALESCE(ncs.ReportFileName,'') as ReportFileName,
		COALESCE(ncs.DssContainer,'') as DssContainer,
		COALESCE(ncs.ReportEndDate,'1900-01-01') as ReportEndDate,
		pub.SourceContainerName,
		pub.SourceFolderKey,
		j.DateTimeCreatedUTC,
		Cast(Case When ct.[Type]  = 'ILR' OR ct.[Type]  = 'ESF' OR ct.[Type]  = 'EAS' THEN 1 ELSE 0 END As Bit) As IsCollectionUploadType,
		esf.ContractReferenceNumber,
		vrd.[Rule],
		CASE WHEN vrd.SelectedCollectionYear IS NULL THEN 0 ELSE vrd.SelectedCollectionYear END As SelectedCollectionYear ,
		j.DateTimeUpdatedUTC

FROM         Job AS j 
			INNER JOIN Collection AS c ON 
				c.CollectionId = j.CollectionId 
			INNER JOIN CollectionType AS ct ON 
				ct.CollectionTypeId = c.CollectionTypeId 
			LEFT OUTER JOIN FileUploadJobMetaData AS meta ON 
				meta.JobId = j.JobId 
            LEFT OUTER JOIN ReturnPeriod AS rp ON 
				rp.CollectionId = c.CollectionId AND rp.PeriodNumber = meta.PeriodNumber 
			LEFT OUTER JOIN
                ReturnPeriodDisplayOverride AS rpd ON 
				rpd.ReturnPeriodId = rp.ReturnPeriodId
			LEFT OUTER JOIN
                NcsJobMetaData AS ncs ON 
				ncs.JobId = j.JobId
			LEFT OUTER JOIN
                EsfJobMetaData AS esf ON 
				esf.JobId = j.JobId
			LEFT OUTER JOIN ReportsPublicationJobMetaData pub ON
				j.JobId = pub.JobId
			LEFT OUTER JOIN ValidationRuleDetailsReportJobMetaData vrd ON
				j.JobId = vrd.JobId
			LEFT OUTER JOIN
                             (SELECT   JobId, MIN(DateTimeSubmittedUtc) AS SubmittedDateTimeUTC
                                FROM         IlrJobMetaData
                                GROUP BY JobId) AS ilr ON j.JobId = ilr.JobId 
                    
							
			             
