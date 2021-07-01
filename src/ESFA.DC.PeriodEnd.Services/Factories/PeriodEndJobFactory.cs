using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Interfaces;

namespace ESFA.DC.PeriodEnd.Services.Factories
{
    public class PeriodEndJobFactory : IPeriodEndJobFactory
    {
        private readonly ICollectionService _collectionService;

        public PeriodEndJobFactory(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        public async Task<FileUploadJob> CreateJobAsync(
            string collectionName,
            int collectionYear,
            int periodNumber,
            long ukPrn = 0,
            string storageReference = "",
            string fileName = "")
        {
            if (string.IsNullOrEmpty(storageReference))
            {
                storageReference = (await _collectionService.GetCollectionFromNameAsync(CancellationToken.None, collectionName))?.StorageReference;
            }

            return new FileUploadJob
            {
                CollectionName = collectionName,
                Priority = 1,
                Status = JobStatusType.Ready,
                CreatedBy = "Period End",
                CollectionYear = collectionYear,
                PeriodNumber = periodNumber,
                Ukprn = ukPrn,
                StorageReference = storageReference,
                FileName = fileName
            };
        }
    }
}