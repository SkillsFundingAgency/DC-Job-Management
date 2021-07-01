using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.DataWarehouse2Path
{
    public class DataWarehouse2Email : AbstractEmailTask, IDataWarehouse2PathItem
    {
        private readonly ILogger _logger;

        public DataWarehouse2Email(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "Data warehouse 2 Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.DataWarehouse2Email;

        public string CollectionNameInDatabase => null;

        public override int EmailPathItemId => EmailIds.DataWarehouse2Email;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.DataWarehouse2Path);

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}