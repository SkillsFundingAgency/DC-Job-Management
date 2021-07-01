using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class ReportFileServiceILR : AbstractReportFileService, IReportFileServiceILR
    {
        private readonly IFileService _fileService;
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly IReturnPeriodService _returnPeriodService;

        public ReportFileServiceILR(
            IFileService fileService,
            IEnumerable<IPathItem> pathItems,
            IPeriodEndRepository periodEndRepository,
            IReturnPeriodService returnPeriodService,
            ILogger logger)
            : base(fileService, pathItems, periodEndRepository, logger)
        {
            _fileService = fileService;
            _periodEndRepository = periodEndRepository;
            _returnPeriodService = returnPeriodService;
        }

        public override char ReturnPeriodPrefix => PeriodEndConstants.ILRReturnPeriodPrefix;

        public async Task<IEnumerable<ReportDetails>> GetReportSamples(
            string container,
            int period,
            CancellationToken cancellationToken)
        {
            var fileName = "Reports.zip";

            var details = new List<ReportDetails>();

            foreach (var ukPrn in PeriodEndConstants.SampleReportProviderUkPrns)
            {
                var fileReference = $"{ReturnPeriodPrefix}{period:D2}/{ukPrn}/{fileName}";

                if (await _fileService.ExistsAsync(fileReference, container, cancellationToken))
                {
                    details.Add(new ReportDetails
                    {
                        DisplayName = $"{ukPrn} Reports",
                        Url = fileReference
                    });
                }
            }

            return details;
        }

        public async Task<IEnumerable<ReportDetails>> GetLLVReportSamples(
            string container,
            int period,
            CancellationToken cancellationToken)
        {
            var details = new List<ReportDetails>();

            foreach (var ukPrn in PeriodEndConstants.SampleReportProviderUkPrns)
            {
                var fileName = PeriodEndConstants.LearnerLevelViewReportFileName.Replace(PeriodEndConstants.UkprnToken, ukPrn.ToString());
                var fileReference = $"{ReturnPeriodPrefix}{period:D2}/{ukPrn}/{fileName}";

                if (await _fileService.ExistsAsync(fileReference, container, cancellationToken))
                {
                    details.Add(new ReportDetails
                    {
                        DisplayName = $"{ukPrn} Reports",
                        Url = fileReference
                    });
                }
            }

            return details;
        }

        public async Task<IEnumerable<ReportDetails>> GetMcaReports(
            string container,
            int collectionYear,
            int period,
            CancellationToken cancellationToken)
        {
            var fileName = "Reports.zip";

            var details = new List<ReportDetails>();

            var ilrCollectionName =
                PeriodEndConstants.CollectionName_ILRSubmission.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString());
            var periodEndDate = await _returnPeriodService.GetPeriodEndEndDate(ilrCollectionName, period);

            var mcaProvider = await _periodEndRepository.GetActiveMcaProvidersAsync(collectionYear, periodEndDate, cancellationToken);

            foreach (var provider in mcaProvider)
            {
                var fileReference = $"{ReturnPeriodPrefix}{period:D2}/{provider.UkPrn}/{fileName}";

                if (await _fileService.ExistsAsync(fileReference, container, cancellationToken))
                {
                    details.Add(new ReportDetails
                    {
                        DisplayName = $"{provider.UkPrn} {provider.Code} Reports",
                        Url = fileReference
                    });
                }
            }

            return details;
        }
    }
}