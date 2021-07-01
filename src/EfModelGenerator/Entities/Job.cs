using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Job
    {
        public Job()
        {
            EasJobMetaData = new HashSet<EasJobMetaData>();
            EsfJobMetaData = new HashSet<EsfJobMetaData>();
            FileUploadJobMetaData = new HashSet<FileUploadJobMetaData>();
            FisJobMetaData = new HashSet<FisJobMetaData>();
            IlrJobMetaData = new HashSet<IlrJobMetaData>();
            NcsJobMetaData = new HashSet<NcsJobMetaData>();
            PathItemJob = new HashSet<PathItemJob>();
            ReportsPublicationJobMetaData = new HashSet<ReportsPublicationJobMetaData>();
            ValidationRuleDetailsReportJobMetaData = new HashSet<ValidationRuleDetailsReportJobMetaData>();
        }

        public long JobId { get; set; }
        public int CollectionId { get; set; }
        public short Priority { get; set; }
        public DateTime DateTimeCreatedUtc { get; set; }
        public DateTime? DateTimeUpdatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public short Status { get; set; }
        public byte[] RowVersion { get; set; }
        public string NotifyEmail { get; set; }
        public short? CrossLoadingStatus { get; set; }
        public long? Ukprn { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ICollection<EasJobMetaData> EasJobMetaData { get; set; }
        public virtual ICollection<EsfJobMetaData> EsfJobMetaData { get; set; }
        public virtual ICollection<FileUploadJobMetaData> FileUploadJobMetaData { get; set; }
        public virtual ICollection<FisJobMetaData> FisJobMetaData { get; set; }
        public virtual ICollection<IlrJobMetaData> IlrJobMetaData { get; set; }
        public virtual ICollection<NcsJobMetaData> NcsJobMetaData { get; set; }
        public virtual ICollection<PathItemJob> PathItemJob { get; set; }
        public virtual ICollection<ReportsPublicationJobMetaData> ReportsPublicationJobMetaData { get; set; }
        public virtual ICollection<ValidationRuleDetailsReportJobMetaData> ValidationRuleDetailsReportJobMetaData { get; set; }
    }
}
