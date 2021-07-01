using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Services
{
    public class DASPaymentsService : IDASPaymentsService
    {
        private readonly DASSettings _settings;

        public DASPaymentsService(DASSettings settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<DasPaymentsModel>> GetMissingDASPayments(
            IEnumerable<CurrentPeriodSuccessfulIlrModel> ilrJobs,
            int year,
            int collectionPeriod,
            CancellationToken cancellationToken)
        {
            // Due to performance issues this has temporarily been removed.
            // using (var connection = new SqlConnection(_settings.DasPaymentsConnectionString))
            // {
            //    await connection.OpenAsync(cancellationToken);
            //    var dasUkprns = await connection.QueryAsync<long>(
            //        $@"SELECT DISTINCT(Ukprn) as Ukprn FROM Payments2.LatestSuccessfulJobs where AcademicYear={year} and CollectionPeriod={collectionPeriod}",
            //        CommandType.Text);

            // var ilrJobsForYearAndPeriod = ilrJobs.Where(j => j.CollectionYear == year && j.PeriodNumber == collectionPeriod);

            // return ilrJobsForYearAndPeriod
            //        .Where(w => !dasUkprns.Contains(w.Ukprn))
            //        .Select(s => new DasPaymentsModel()
            //            { JobId = s.JobId, Ukprn = s.Ukprn });
            // }

            return new List<DasPaymentsModel>();
        }
    }
}