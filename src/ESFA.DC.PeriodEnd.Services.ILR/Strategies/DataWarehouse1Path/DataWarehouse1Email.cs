using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.DataWarehouse1Path
{
    public class DataWarehouse1Email : AbstractEmailTask, IDataWarehouse1PathItem
    {
        private readonly ILogger _logger;

        public DataWarehouse1Email(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
        : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "Data warehouse 1 Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.DataWarehouse1Email;

        public string CollectionNameInDatabase => null;

        public override int EmailPathItemId => EmailIds.DataWarehouse1Email;

        protected override int PathId => (int)PeriodEndPath.DataWarehouse1Path;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}