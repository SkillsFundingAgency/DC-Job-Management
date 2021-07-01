using System.Collections.Generic;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class ReportFileServiceNCS : AbstractReportFileService, IReportFileServiceNCS
    {
        public ReportFileServiceNCS(
            IFileService fileService,
            IEnumerable<INCSPathItem> pathItems,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
            : base(fileService, pathItems, periodEndRepository, logger)
        {
        }

        public override char ReturnPeriodPrefix => PeriodEndConstants.NCSReturnPeriodPrefix;
    }
}