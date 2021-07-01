using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Services
{
    public class ReturnCodeService : IReturnCodeService
    {
        private readonly IESFReturnPeriodHelper _esfCollectionMonthHelper;
        private readonly IAppsReturnPeriodHelper _appsReturnPeriodHelper;

        public ReturnCodeService(
            IESFReturnPeriodHelper esfCollectionMonthHelper,
            IAppsReturnPeriodHelper appsReturnPeriodHelper)
        {
            _esfCollectionMonthHelper = esfCollectionMonthHelper;
            _appsReturnPeriodHelper = appsReturnPeriodHelper;
        }

        public async Task<CollectionPeriod> GetReturnCodeForPeriodAsync(string collectionType, int year, int period)
        {
            var currentPeriod = await GetCurrentPeriod(collectionType, year, period);

            return GetCollectionPeriod(collectionType, year, currentPeriod);
        }

        public async Task<CollectionPeriod> GetReturnCodeForPreviousPeriodAsync(string collectionType, int year, int period)
        {
            var currentPeriod = await GetCurrentPeriod(collectionType, year, period);
            var previousPeriod = currentPeriod - 1;

            return GetCollectionPeriod(collectionType, year, previousPeriod);
        }

        private async Task<int> GetCurrentPeriod(string collectionType, int year, int period)
        {
            if (collectionType == CollectionTypeConstants.Esf)
            {
                period = await _esfCollectionMonthHelper.GetESFReturnPeriod(year, period);
            }
            else if (collectionType == CollectionTypeConstants.Apps)
            {
                period = await _appsReturnPeriodHelper.GetReturnPeriod(year, period);
            }

            return period;
        }

        private CollectionPeriod GetCollectionPeriod(string collectionType, int year, int period)
        {
            switch (collectionType)
            {
                case CollectionTypeConstants.Ilr:
                    return new CollectionPeriod { Collection = $"{CollectionTypeConstants.Ilr}{year}", Period = $"R{period:D2}" };
                case CollectionTypeConstants.NCS:
                    return new CollectionPeriod { Collection = $"{CollectionTypeConstants.NCS}{year}", Period = $"N{period:D2}" };
                case CollectionTypeConstants.Esf:
                    return new CollectionPeriod { Collection = CollectionTypeConstants.Esf, Period = $"{CollectionTypeConstants.Esf}{period:D2}" };
                case CollectionTypeConstants.Apps:
                    return new CollectionPeriod { Collection = CollectionTypeConstants.Apps, Period = $"{CollectionTypeConstants.Apps}{period:D2}" };
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectionType), $"{collectionType} not a recognised collection type");
            }
        }
    }
}
