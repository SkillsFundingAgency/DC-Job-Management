using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractHiddenPathItem : IILRPathItem, INCSPathItem
    {
        private readonly ILogger _logger;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;
        private readonly IPeriodEndRepository _periodEndRepository;

        protected AbstractHiddenPathItem(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IPeriodEndRepository periodEndRepository)
        {
            _logger = logger;
            _returnFactory = returnFactory;
            _stateService = stateService;
            _periodEndRepository = periodEndRepository;
        }

        public abstract List<int> ItemSubPaths { get; }

        public abstract int PathItemId { get; }

        public virtual string DisplayName { get; }

        public virtual string ReportFileName { get; }

        public virtual string CollectionNameInDatabase => CollectionName;

        public virtual bool IsPausing => false;

        public virtual bool IsInitiating => false;

        public virtual bool IsHidden => true;

        public virtual PeriodEndEntityType EntityType => PeriodEndEntityType.PathItem;

        public virtual int EmailPathItemId { get; }

        protected bool IsBlockingTask => false;

        protected abstract int PathId { get; }

        protected abstract string CollectionName { get; }

        public virtual Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsValidForPeriod(
            int period,
            int collectionYear,
            IDictionary<string, IEnumerable<int>> validities)
        {
            return await _periodEndRepository.GetValidityForSubPathAsync(ItemSubPaths.First(), collectionYear, period, CancellationToken.None);
        }

        protected async Task<PathItemReturn> ExecuteAsync(int ordinal, int collectionYear, int period)
        {
            _logger.LogInfo($"In {GetType().Name}");

            try
            {
                await _stateService.SavePathItem(
                    new PathItemModel
                    {
                        PathId = PathId,
                        Ordinal = ordinal,
                        Name = DisplayName,
                        IsPausing = IsPausing,
                        HasJobs = false,
                        PathItemJobs = Enumerable.Empty<PathItemJobModel>().ToList()
                    },
                    collectionYear,
                    period);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return _returnFactory.CreatePathTaskReturn(IsBlockingTask, Enumerable.Empty<long>(), ItemSubPaths);
        }

        private string GetValidityKey()
        {
            return $"{PathItemId}-{EntityType}";
        }
    }
}