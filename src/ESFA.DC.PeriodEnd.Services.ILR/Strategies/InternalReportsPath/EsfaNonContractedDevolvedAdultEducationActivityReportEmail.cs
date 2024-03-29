﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.InternalReportsPath
{
    public class EsfaNonContractedDevolvedAdultEducationActivityReportEmail : AbstractBlockingEmailTask, IInternalReportsPathItem
    {
        private readonly ILogger _logger;

        public EsfaNonContractedDevolvedAdultEducationActivityReportEmail(
            IEmailService emailService,
            ILogger logger,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
            _logger = logger;
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "Period End ESFA Non-Contracted Devolved Adult Education Activity Report Email";

        public string ReportFileName => null;

        public override int PathItemId => PeriodEndPathItem.EsfaNonContractedDevolvedAdultEducationActivityReportEmail;

        public string CollectionNameInDatabase => null;

        public override int EmailPathItemId => EmailIds.ESFANonContractedDevolvedAdultEducationActivityReportEmail;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.InternalReportsPath);

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            return await base.ExecuteAsync(pathItemParams);
        }
    }
}