using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IReportFileServiceILR : IReportFileService
    {
        Task<IEnumerable<ReportDetails>> GetReportSamples(
            string container,
            int period,
            CancellationToken cancellationToken);

        Task<IEnumerable<ReportDetails>> GetMcaReports(
            string container,
            int collectionYear,
            int period,
            CancellationToken cancellationToken);

        Task<IEnumerable<ReportDetails>> GetLLVReportSamples(
            string container,
            int period,
            CancellationToken cancellationToken);
    }
}
