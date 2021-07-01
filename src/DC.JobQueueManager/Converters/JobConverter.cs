using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager.Converters
{
    public partial class JobConverter : IJobConverter
    {
        private readonly ICollectionService _collectionService;

        public JobConverter(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        public async Task<Data.Entities.Job> Convert(Jobs.Model.Job source)
        {
            if (source == null)
            {
                return null;
            }

            var entity = new Data.Entities.Job();
            await Convert(source, entity);
            return entity;
        }

        public async Task<Jobs.Model.Job> Convert(Data.Entities.Job source)
        {
            if (source == null)
            {
                return null;
            }

            var entity = new Jobs.Model.Job();
            await Convert(source, entity);
            return entity;
        }

        public async Task Convert(Jobs.Model.Job source, Data.Entities.Job destination)
        {
            destination.DateTimeCreatedUtc = source.DateTimeCreatedUtc;
            destination.Priority = source.Priority;
            destination.Status = (short)source.Status;
            destination.DateTimeUpdatedUtc = source.DateTimeUpdatedUtc;
            destination.JobId = source.JobId;
            destination.CreatedBy = source.CreatedBy;
            destination.NotifyEmail = source.NotifyEmail;
            destination.RowVersion = source.RowVersion == null ? null : System.Text.Encoding.UTF8.GetBytes(source.RowVersion);
            destination.CrossLoadingStatus = source.CrossLoadingStatus.HasValue ? (short)source.CrossLoadingStatus : (short?)null;
            destination.Ukprn = source.Ukprn;
            destination.CollectionId = (await _collectionService.GetCollectionFromNameAsync(CancellationToken.None, source.CollectionName)).CollectionId;
        }

        public async Task Convert(Data.Entities.Job source, Jobs.Model.Job destination)
        {
            destination.DateTimeCreatedUtc = source.DateTimeCreatedUtc;
            destination.Priority = source.Priority;
            destination.Status = (JobStatusType)source.Status;
            destination.DateTimeUpdatedUtc = source.DateTimeUpdatedUtc;
            destination.JobId = source.JobId;
            destination.RowVersion = source.RowVersion == null ? null : System.Convert.ToBase64String(source.RowVersion);
            destination.CreatedBy = source.CreatedBy;
            destination.NotifyEmail = source.NotifyEmail;
            destination.CollectionName = source.Collection?.Name;
            destination.CrossLoadingStatus = source.CrossLoadingStatus.HasValue ? (JobStatusType)source.CrossLoadingStatus.Value : (JobStatusType?)null;
            destination.Ukprn = source.Ukprn.GetValueOrDefault();
            destination.DateTimeCreatedUtc = source.DateTimeCreatedUtc;
        }

        public async Task Convert(Data.Entities.Job source, FileUploadJob destination)
        {
            destination.DateTimeSubmittedUtc = source.DateTimeCreatedUtc;
            destination.Priority = source.Priority;
            destination.Status = (JobStatusType)source.Status;
            destination.DateTimeUpdatedUtc = source.DateTimeUpdatedUtc;
            destination.JobId = source.JobId;
            destination.RowVersion = source.RowVersion == null ? null : System.Convert.ToBase64String(source.RowVersion);
            destination.CreatedBy = source.CreatedBy;
            destination.NotifyEmail = source.NotifyEmail;
            destination.CollectionName = source.Collection.Name;
            destination.CrossLoadingStatus = source.CrossLoadingStatus.HasValue ? (JobStatusType)source.CrossLoadingStatus.Value : (JobStatusType?)null;
            destination.Ukprn = source.Ukprn.GetValueOrDefault();
            destination.DateTimeCreatedUtc = source.DateTimeCreatedUtc;
        }

        public async Task Convert(FileUploadJobMetaData source, Provider providerSource, FileUploadJob destination)
        {
            destination.ProviderName = providerSource.Name;
            await Convert(source, destination);
        }

        public async Task Convert(FileUploadJobMetaData source, FileUploadJob destination)
        {
            if (source == null)
            {
                return;
            }

            if (destination == null)
            {
                destination = new FileUploadJob();
            }

            destination.FileName = source.FileName;
            destination.FileSize = source.FileSize.GetValueOrDefault(0);
            destination.StorageReference = source.StorageReference;
            destination.JobId = source.JobId;
            destination.CollectionName = source.Job.Collection.Name;
            destination.PeriodNumber = source.PeriodNumber;
            destination.Ukprn = source.Job.Ukprn.GetValueOrDefault();
            await Convert(source.Job, destination);
        }

        public async Task Convert(NcsJobMetaData source, NcsJob destination)
        {
            if (source == null)
            {
                return;
            }

            if (destination == null)
            {
                destination = new NcsJob();
            }

            // Ncs Meta data
            destination.ExternalJobId = source.ExternalJobId;
            destination.TouchpointId = source.TouchpointId;
            destination.ExternalTimestamp = source.ExternalTimestamp;
            destination.ReportFileName = source.ReportFileName;
            destination.DssContainer = source.DssContainer;

            // Job data
            destination.JobId = source.JobId;
            destination.CollectionName = source.Job.Collection.Name;
            destination.Status = (JobStatusType)source.Job.Status;
            destination.Priority = source.Job.Priority;
            destination.DateTimeCreatedUtc = source.Job.DateTimeCreatedUtc;
            destination.DateTimeUpdatedUtc = source.Job.DateTimeUpdatedUtc;
            destination.RowVersion = source.Job.RowVersion == null ? null : System.Convert.ToBase64String(source.Job.RowVersion);
            destination.CreatedBy = source.Job.CreatedBy;
            destination.NotifyEmail = source.Job.NotifyEmail;
            destination.CrossLoadingStatus = source.Job.CrossLoadingStatus.HasValue ? (JobStatusType)source.Job.CrossLoadingStatus.Value : (JobStatusType?)null;
            destination.Ukprn = source.Job.Ukprn.GetValueOrDefault();
        }

        public async Task Convert(FileUploadJobMetaData source, SubmittedJob destination)
        {
            if (source == null)
            {
                return;
            }

            if (destination == null)
            {
                destination = new SubmittedJob();
            }

            destination.FileName = source.FileName;
            destination.JobId = source.JobId;
            destination.CollectionName = source.Job.Collection.Name;
            destination.PeriodNumber = source.PeriodNumber;
            destination.Ukprn = source.Job.Ukprn.GetValueOrDefault();
        }
    }
}