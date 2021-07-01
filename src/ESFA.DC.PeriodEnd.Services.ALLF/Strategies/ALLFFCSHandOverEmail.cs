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

namespace ESFA.DC.PeriodEnd.Services.ALLF.Strategies
{
    public class ALLFFCSHandOverEmail : AbstractEmailTask, IALLFPathItem
    {
        private readonly ILogger _logger;

        public ALLFFCSHandOverEmail(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "FCS Hand Over Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.ALLFFCSHandoverEmail;

        public string CollectionNameInDatabase => null;

        public override int EmailPathItemId => EmailIds.ALLFFCSHandoverEmail;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ALLFCriticalPath);

        protected override string PeriodPrefix => PeriodEndConstants.AllfPeriodPrefix;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}