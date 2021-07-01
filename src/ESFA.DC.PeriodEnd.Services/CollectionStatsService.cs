using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Utils;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.PeriodEnd.Services
{
    public class CollectionStatsService : ICollectionStatsService
    {
        private readonly IFileService _fileService;
        private readonly ILogger _logger;
        private readonly IInternalReportsPathItem _internalReportsPathItem;
        private readonly ISerializationService _serializationService;
        private string _fullReportPath;

        public CollectionStatsService(
            IFileService fileService,
            ILogger logger,
            IEnumerable<IInternalReportsPathItem> internalReportsPathItem,
            ISerializationService serializationService)
        {
            _fileService = fileService;
            _logger = logger;
            _internalReportsPathItem = internalReportsPathItem.FirstOrDefault(x => x.PathItemId == PeriodEndPathItem.CollectionStats);
            _serializationService = serializationService;
        }

        public async Task<IEnumerable<CollectionStats>> GetCollectionStats(
            string container,
            int period,
            CancellationToken cancellationToken)
        {
            _logger.LogVerbose($"Entered GetCollectionStats for Period:{period}");

            _fullReportPath = $"R{period:D2}\\{_internalReportsPathItem.ReportFileName}";

            if (!await _fileService.ExistsAsync(_fullReportPath, container, cancellationToken))
            {
                _logger.LogInfo($"Collection Stats file for Period:{period} not found");
                return Enumerable.Empty<CollectionStats>();
            }

            _logger.LogVerbose($"Exit GetCollectionStats for Period:{period}");
            return await DeserializeFromStorage(container, cancellationToken);
        }

        private async Task<IEnumerable<CollectionStats>> DeserializeFromStorage(string container, CancellationToken cancellationToken)
        {
            var fileStream = await _fileService.OpenReadStreamAsync(_fullReportPath, container, cancellationToken);
            return _serializationService.Deserialize<IEnumerable<CollectionStats>>(fileStream);
        }
    }
}