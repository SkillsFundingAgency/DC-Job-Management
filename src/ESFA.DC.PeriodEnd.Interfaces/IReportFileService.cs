using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IReportFileService
    {
        Task<IEnumerable<ReportDetails>> GetReportDetails(
            string container,
            int period,
            CancellationToken cancellationToken,
            int topReports = 1);
    }
}