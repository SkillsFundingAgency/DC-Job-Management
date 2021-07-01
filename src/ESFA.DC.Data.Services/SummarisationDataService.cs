using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Utils;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Data.Services
{
    public class SummarisationDataService : ISummarisationDataService
    {
        private const int DefaultCollectionsCount = 2;
        private const int LastPeriodInYear = 14;

        private readonly Func<ISummarisationContext> _summarisationFactory;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IReturnCodeService _returnCodeService;

        public SummarisationDataService(
            Func<ISummarisationContext> summarisationFactory,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            IValidityPeriodService validityPeriodService,
            IReturnCodeService returnCodeService)
        {
            _summarisationFactory = summarisationFactory;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _validityPeriodService = validityPeriodService;
            _returnCodeService = returnCodeService;
        }

        /// <summary>
        /// Returns collection codes for a given collection type - default collection
        /// </summary>
        /// <param name="collectionType"></param>
        /// <param name="maxCollectionsCodesCount"></param>
        /// <param name="dateTimeUntil"></param>
        /// <returns></returns>
        public async Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(
            string collectionType,
            CancellationToken cancellationToken,
            int? maxCollectionsCodesCount = null,
            DateTime? dateTimeUntil = null)
        {
            List<SummarisationCollectionReturnCode> result = null;

            try
            {
                dateTimeUntil = dateTimeUntil ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
                maxCollectionsCodesCount = maxCollectionsCodesCount ?? DefaultCollectionsCount;

                using (var context = _summarisationFactory())
                {
                    result = await context.CollectionReturns
                        .Where(x => x.DateTime <= dateTimeUntil &&
                                    (string.IsNullOrEmpty(collectionType) ||
                                     x.CollectionType.Equals(collectionType, StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(x => x.DateTime.GetValueOrDefault())
                        .Select(item => new SummarisationCollectionReturnCode()
                        {
                            CollectionType = item.CollectionType,
                            CollectionReturnCode = item.CollectionReturnCode,
                            DateTime = item.DateTime.GetValueOrDefault(),
                            Id = item.Id,
                        })
                        .Take(maxCollectionsCodesCount.Value)
                        .ToListAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Error occured trying to get data for collection return codes for collection type : {collectionType}, collectionsCount : {maxCollectionsCodesCount}, datetime : {dateTimeUntil}",
                    e);
            }

            return result;
        }

        public async Task<List<SummarisationCollectionReturnCode>> GetSummarisationCollectionCodesAsync(
            string collectionType,
            string collectionReturnCode,
            string previousCollectionReturnCode,
            CancellationToken cancellationToken)
        {
            var result = new List<SummarisationCollectionReturnCode>();

            try
            {
                using (var context = _summarisationFactory())
                {
                    result = await context.CollectionReturns
                        .Where(cr => cr.CollectionType == collectionType &&
                                     (cr.CollectionReturnCode == collectionReturnCode ||
                                      (previousCollectionReturnCode != null &&
                                       cr.CollectionReturnCode == previousCollectionReturnCode)))
                        .OrderBy(cr => cr.CollectionReturnCode)
                        .Select(item => new SummarisationCollectionReturnCode()
                        {
                            CollectionType = item.CollectionType,
                            CollectionReturnCode = item.CollectionReturnCode,
                            DateTime = item.DateTime.GetValueOrDefault(),
                            Id = item.Id,
                        })
                        .ToListAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Error occured trying to get data for collection return code for collection type : {collectionType}, collectionReturnCode : {collectionReturnCode}",
                    e);
            }

            return result;
        }

        public async Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(
            List<int> collectionReturnIds,
            CancellationToken cancellationToken)
        {
            List<SummarisationTotal> result = null;
            try
            {
                using (var context = _summarisationFactory())
                {
                    result = await context.SummarisedActuals
                        .Where(x => collectionReturnIds.Contains(x.CollectionReturnId))
                        .GroupBy(grp => new
                        {
                            grp.CollectionReturnId, grp.CollectionReturn.CollectionType,
                            grp.CollectionReturn.CollectionReturnCode
                        })
                        .Select(g => new SummarisationTotal()
                        {
                            CollectionReturnId = g.Key.CollectionReturnId,
                            TotalActualValue = g.Sum(si => si.ActualValue),
                            CollectionType = g.Key.CollectionType,
                            CollectionReturnCode = g.Key.CollectionReturnCode,
                        })
                        .ToListAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get data for Summarisation totals", ex);
            }

            return result;
        }

        public async Task<List<SummarisationCollectionReturnCode>> GetReturnCodeForPeriodAsync(
            string collectionType,
            int year,
            int period,
            int pathItemId,
            CancellationToken cancellationToken)
        {
            var periodDetails = await _returnCodeService.GetReturnCodeForPeriodAsync(collectionType, year, period);

            var previousYear = period == 1 && collectionType != CollectionTypeConstants.Ilr ? GetPreviousYear(year) : year;

            var validityKey = $"{pathItemId}-{PeriodEndEntityType.PathItem}";
            var allValidities = await _validityPeriodService.GetValidPeriodsForCollections(year);

            CollectionPeriod previousPeriodDetails;
            if (allValidities.TryGetValue(validityKey, out var validities))
            {
                var previousPeriod = validities.OrderByDescending(v => v).FirstOrDefault(v => v < period);

                if (previousYear != year && collectionType != CollectionTypeConstants.Ilr)
                {
                    var previousValidities = await _validityPeriodService.GetValidPeriodsForCollections(previousYear);
                    if (previousValidities.TryGetValue(validityKey, out var previousYearvalidities))
                    {
                        previousPeriod = previousYearvalidities.OrderByDescending(v => v).FirstOrDefault(v => v < LastPeriodInYear);
                    }
                }

                previousPeriodDetails = await _returnCodeService.GetReturnCodeForPeriodAsync(collectionType, previousYear, previousPeriod);
            }
            else
            {
                previousPeriodDetails = await _returnCodeService.GetReturnCodeForPreviousPeriodAsync(collectionType, year, period);
            }

            var codes = await GetSummarisationCollectionCodesAsync(periodDetails.Collection, periodDetails.Period, previousPeriodDetails.Period, cancellationToken);

            codes.ForEach(c =>
            {
                c.CurrentReturnCode = periodDetails.Period;
            });

            return codes;
        }

        private int GetPreviousYear(int currentYear)
        {
            var firstPart = Convert.ToInt32(currentYear.ToString().Substring(0, 2)) - 1;
            var secondPart = Convert.ToInt32(currentYear.ToString().Substring(2)) - 1;

            return Convert.ToInt32($"{firstPart}{secondPart}");
        }
    }
}
