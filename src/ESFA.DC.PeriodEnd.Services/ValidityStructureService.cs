using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class ValidityStructureService : IValidityStructureService
    {
        private readonly IPathStructureServiceILR _pathStructureService;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IValidityPeriodRepository _validityPeriodRepository;
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly IDASStoppingPathItem _dasStop;

        public ValidityStructureService(
            IPathStructureServiceILR pathStructureService,
            IValidityPeriodService validityPeriodService,
            IValidityPeriodRepository validityPeriodRepository,
            IPeriodEndRepository periodEndRepository,
            IDASStoppingPathItem dasStop)
        {
            _pathStructureService = pathStructureService;
            _validityPeriodService = validityPeriodService;
            _validityPeriodRepository = validityPeriodRepository;
            _periodEndRepository = periodEndRepository;
            _dasStop = dasStop;
        }

        public async Task<PeriodEndValidity> GetAllPeriodEndItems(YearPeriod period, CancellationToken cancellationToken)
        {
            if (period == null)
            {
                throw new System.ArgumentException("period can not be null");
            }

            var periodEndValidity = new PeriodEndValidity();
            var paths = await _pathStructureService.GetPathStructures(period, null, null, null, true, true, cancellationToken);
            var pathValidityPeriods = await _validityPeriodRepository.GetPathValidityPeriods(period.Year, period.Period, cancellationToken);

            var emailValidities = await _validityPeriodRepository.GetEmailValidities(period.Year, period.Period, cancellationToken);

            var pathItemValidityPeriods =
                await _validityPeriodRepository.GetValidityPeriodList(period.Year, period.Period, cancellationToken);

            foreach (var path in paths)
            {
                path.IsCritical = path.PathId == (int)PeriodEndPath.ILRCriticalPath;
                pathValidityPeriods.TryGetValue(path.PathId, out var validityModel);

                path.IsValidForPeriod = validityModel?.Enabled ?? false ? PeriodEndValidityState.Checked : PeriodEndValidityState.Unchecked;

                var periodEndRunning = await _periodEndRepository.IsPeriodEndRunning(
                    period.Year,
                    period.Period,
                    PeriodEndConstants.IlrCollectionNamePrefix,
                    cancellationToken);

                periodEndValidity.PeriodEndIsRunning = periodEndRunning;

                AddNonPathRelatedPathItems(path);

                foreach (var pathItem in path.PathItems)
                {
                    switch (pathItem.EntityType)
                    {
                        case PeriodEndEntityType.PathItem:
                            var validity = pathItemValidityPeriods.SingleOrDefault(vp => vp.PathItemId == pathItem.PathItemId);
                            pathItem.IsValidForPeriod = IsItemValid(validity);
                            break;
                        case PeriodEndEntityType.Email:
                            emailValidities.TryGetValue(pathItem.PathItemId, out var emailValidity);
                            pathItem.IsValidForPeriod = IsItemValid(emailValidity);
                            break;
                    }
                }
            }

            periodEndValidity.PeriodEndHasRunForPeriod = await _validityPeriodService
                    .HasPeriodEndRunForPeriodAsync(
                        PeriodEndConstants.IlrCollectionNamePrefix,
                        period.Year,
                        period.Period,
                        cancellationToken);
            periodEndValidity.Paths = paths;

            return periodEndValidity;
        }

        private void AddNonPathRelatedPathItems(PathPathItemsModel path)
        {
            if (path.IsCritical)
            {
                path.PathItems = path.PathItems.Concat(new[]
                {
                    new PathItemModel
                    {
                        Name = _dasStop.DisplayName,
                        PathItemId = _dasStop.PathItemId,
                        EntityType = PeriodEndEntityType.PathItem
                    }
                });
            }
        }

        private PeriodEndValidityState IsItemValid(ValidityPeriodLookupModel validity)
        {
            if (validity == null)
            {
                return PeriodEndValidityState.Disabled;
            }

            if (validity.Enabled ?? false)
            {
                return PeriodEndValidityState.Checked;
            }

            return PeriodEndValidityState.Unchecked;
        }
    }
}