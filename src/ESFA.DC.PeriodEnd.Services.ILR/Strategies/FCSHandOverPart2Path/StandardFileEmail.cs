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

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart2Path
{
    public class StandardFileEmail : AbstractEmailTask, IFCSHandOverPart2PathItem
    {
        private readonly ILogger _logger;

        public StandardFileEmail(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "Standard File Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.StandardFileEmail;

        public string CollectionNameInDatabase => null;

        public override int EmailPathItemId => EmailIds.StandardFileEmail;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath2);

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}