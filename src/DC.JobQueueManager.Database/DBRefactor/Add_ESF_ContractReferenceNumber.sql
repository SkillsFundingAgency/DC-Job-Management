Insert Into EsfJobMetaData
(JobId, ContractReferenceNumber)

Select j.JobId,
Case CollectionId When 3 Then Substring(filename,28,8) Else Substring(filename,29,8) End
from Job j inner join fileuploadjobmetadata meta
on meta.jobId= j.jobId
where collectionId in (3,7,9)
And j.JobId Not in (Select JobId from EsfJobMetaData)