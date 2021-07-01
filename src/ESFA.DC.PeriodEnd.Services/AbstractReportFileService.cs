using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Services
{
    public abstract class AbstractReportFileService : IReportFileService
    {
        private readonly IFileService _fileService;
        private readonly IEnumerable<IPathItem> _pathItems;
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly ILogger _logger;

        public AbstractReportFileService(
            IFileService fileService,
            IEnumerable<IPathItem> pathItems,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
        {
            _fileService = fileService;
            _pathItems = pathItems;
            _periodEndRepository = periodEndRepository;
            _logger = logger;
        }

        public abstract char ReturnPeriodPrefix { get; }

        public async Task<IEnumerable<ReportDetails>> GetReportDetails(
            string container,
            int period,
            CancellationToken cancellationToken,
            int topReports = 1)
        {
            var periodString = $"{ReturnPeriodPrefix}{period:D2}";
            var details = new List<ReportDetails>();

            try
            {
                var fileReferences = (await _fileService.GetFileReferencesAsync(container, periodString, true, cancellationToken)).ToList();

                foreach (var item in _pathItems)
                {
                    if (item.ReportFileName == null)
                    {
                        continue;
                    }

                    var foundFiles = new List<string>();
                    foreach (var fileReference in fileReferences)
                    {
                        if (fileReference.IndexOf(item.ReportFileName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            foundFiles.Add(fileReference);
                        }
                    }

                    var latestFileReferences = foundFiles
                        .OrderByDescending(ff =>
                        {
                            var match = RegexDefinitions.ReportDate.Match(ff);
                            return match.Value;
                        })
                        .Take(topReports);

                    foreach (var latestFileReference in latestFileReferences)
                    {
                        details.Add(new ReportDetails
                        {
                            DisplayName = item.DisplayName,
                            Url = latestFileReference
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return details;
        }
    }
}
