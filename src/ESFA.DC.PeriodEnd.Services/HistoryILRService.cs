using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class HistoryILRService : AbstractHistoryService, IHistoryILRService
    {
        public HistoryILRService(
            IStateService stateService,
            IPeriodEndRepository periodEndRepository,
            IPeriodEndDateTimeService periodEndDateTimeService)
        : base(stateService, periodEndRepository, periodEndDateTimeService)
        {
        }

        public override string CollectionNamePrefix => PeriodEndConstants.IlrCollectionNamePrefix;
    }
}
