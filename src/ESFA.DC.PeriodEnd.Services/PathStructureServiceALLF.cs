using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class PathStructureServiceALLF : AbstractPathStructureService, IPathStructureServiceALLF
    {
        public PathStructureServiceALLF(
            IEnumerable<IPathControllerALLF> controllers,
            IStateService stateService,
            IValidityPeriodService validityPeriodService,
            IPeriodEndOutputService periodEndOutputService,
            IDateTimeProvider dateTimeProvider)
        : base(controllers, stateService, validityPeriodService, periodEndOutputService, dateTimeProvider)
        {
        }

        protected override int CriticalPathId => Convert.ToInt32(PeriodEndPath.ALLFCriticalPath);
    }
}