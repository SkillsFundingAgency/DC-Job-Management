using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public class McaDetailService : IMcaDetailService
    {
        private const string _mcaReportsCollectionName = "PE-MCA-Reports{CollectionYear}";
        private readonly string _reportFileName = "{0}/{1}/DevolvedFundingSummaryReport {2}.csv";

        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public McaDetailService(Func<IJobQueueDataContext> contextFactory, IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<long?> GetLastProcessedJobIdAsync(long ukprn, int period, int collectionYear)
        {
            var collectionName = _mcaReportsCollectionName.Replace("{CollectionYear}", collectionYear.ToString());

            using (IJobQueueDataContext context = _contextFactory())
            {
                var job = await context.Job.Include(x => x.FileUploadJobMetaData)
                    .Include(x => x.Collection)
                    .Where(x => x.Ukprn == ukprn &&
                               x.Collection.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase) &&
                               x.Status == (short)JobStatusType.Completed &&
                               x.Collection.CollectionYear == collectionYear &&
                               x.FileUploadJobMetaData.Any(y => y.JobId == x.JobId && y.PeriodNumber < period))
                    .OrderByDescending(x => x.JobId)
                    .FirstOrDefaultAsync();

                if (job != null)
                {
                    return job.JobId;
                }
            }

            return null;
        }

        public async Task<string> GetGLACodeAsync(long ukprn)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                return (await context.Mcadetail.SingleOrDefaultAsync(x => x.Ukprn == ukprn))?.Glacode;
            }
        }

        public async Task<IEnumerable<long>> GetActiveMcaListAsync(int collectionYear, DateTime periodEndDate)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                return await context.Mcadetail
                    .Where(m => m.AcademicYearFrom <= collectionYear &&
                                (m.AcademicYearTo == null || m.AcademicYearTo >= collectionYear))
                    .Where(m => m.EffectiveFrom <= periodEndDate &&
                                (m.EffectiveTo == null || m.EffectiveTo >= periodEndDate))
                    .Select(m => m.Ukprn)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetGLACodesAsync()
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                return await context.Mcadetail.Select(x => x.Glacode).ToListAsync();
            }
        }
    }
}
