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

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart1Path
{
    public class FCSTeamHandOverPart1Email : AbstractEmailTask, IFCSHandOverPart1PathItem
    {
        private readonly ILogger _logger;

        public FCSTeamHandOverPart1Email(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "FCS Team Hand Over Part 1 Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.FCSTeamHandoverPart1Email;

        public string CollectionNameInDatabase => null;

        public override bool IsPausing => true;

        public override int EmailPathItemId => EmailIds.FCSTeamHandoverPart1Email;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath1);

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}