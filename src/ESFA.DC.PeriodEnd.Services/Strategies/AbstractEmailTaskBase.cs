using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractEmailTaskBase
    {
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPathItemReturnFactory _returnFactory;

        protected AbstractEmailTaskBase(
            ILogger logger,
            IEmailService emailService,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
        {
            _logger = logger;
            _emailService = emailService;
            _stateService = stateService;
            _returnFactory = returnFactory;
        }

        public virtual string DisplayName => null;

        public abstract int PathItemId { get; }

        public virtual bool IsHidden => false;

        public virtual bool IsInitiating => false;

        public virtual PeriodEndEntityType EntityType => PeriodEndEntityType.Email;

        public virtual int EmailPathItemId => 0;

        public virtual int ParentPathItemId => 0;

        public virtual bool IsPausing => false;

        protected virtual bool IsBlockingTask => false;

        protected virtual string TemplateId { get; set; }

        protected abstract int PathId { get; }

        protected virtual string PeriodPrefix => PeriodEndConstants.IlrPeriodPrefix;

        public virtual async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            var parameters = new Dictionary<string, dynamic>
            {
                { "ReturnPeriod", $"{PeriodPrefix}{pathItemParams.Period:D2}" },
                { "CollectionYear", $"{pathItemParams.CollectionYear:D4}" },
                { "Period", $"{pathItemParams.Period:D}" }
            };

            await _emailService.SendEmail(EmailPathItemId, parameters);

            try
            {
                await _stateService.SavePathItem(
                    new PathItemModel
                    {
                        PathId = PathId,
                        Ordinal = pathItemParams.Ordinal,
                        Name = DisplayName,
                        IsPausing = IsPausing,
                        HasJobs = false,
                        PathItemJobs = new List<PathItemJobModel>()
                    },
                    pathItemParams.CollectionYear,
                    pathItemParams.Period);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return _returnFactory.CreatePathTaskReturn(IsBlockingTask, Enumerable.Empty<long>(), Enumerable.Empty<int>());
        }

        public async Task<bool> IsValidForPeriod(
            int period,
            int collectionYear,
            IDictionary<string, IEnumerable<int>> validities)
        {
            if (!validities.TryGetValue(GetValidityKey(), out var validPeriods))
            {
                return false;
            }

            return validPeriods.Contains(period);
        }

        private string GetValidityKey()
        {
            return $"{EmailPathItemId}-{PeriodEndEntityType.Email}";
        }
    }
}