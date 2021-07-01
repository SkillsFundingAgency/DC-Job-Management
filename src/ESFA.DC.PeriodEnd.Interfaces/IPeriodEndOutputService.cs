using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndOutputService
    {
        Task CheckProviderReportJobs(List<PathPathItemsModel> pathModels, int collectionYear, int periodNumber, string collectionType);

        Task PublishProviderReports(int collectionYear, int periodNumber, string collectionType);

        Task PublishFm36Reports(int collectionYear, int periodNumber, string collectionType);

        Task PublishMcaReports(int collectionYear, int periodNumber, string collectionType);

        Task CheckCriticalPath(
            PeriodEndState parentStateModel,
            PathPathItemsModel pathModel,
            int periodYear,
            int periodPeriod,
            string collectionType);
    }
}