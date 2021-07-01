using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public abstract class BaseMultiProviderPeriodEndReport : IILRPathItem
    {
        private readonly IQueryService _queryService;
        private readonly IFileUploadJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;

        protected BaseMultiProviderPeriodEndReport(
            IQueryService queryService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
        {
            _queryService = queryService;
            _jobManager = jobManager;
            _logger = logger;
            _jobFactory = jobFactory;
            _returnFactory = returnFactory;
            _stateService = stateService;
        }

        public abstract List<int> ItemSubPaths { get; }

        public virtual string DisplayName => null;

        public virtual string ReportFileName => null;

        public virtual string CollectionNameInDatabase => CollectionName;

        public abstract int PathItemId { get; }

        public virtual int EmailPathItemId { get; }

        public virtual bool IsPausing { get; } = true;

        public virtual bool IsHidden => false;

        public virtual bool IsInitiating => false;

        public virtual PeriodEndEntityType EntityType => PeriodEndEntityType.PathItem;

        protected virtual int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected virtual string CollectionName => null;

        public virtual async Task<bool> IsValidForPeriod(
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

        public async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            var jobIds = new List<long>();

            var pathItem = new PathItemModel
            {
                Name = DisplayName,
                PathId = PathId,
                Ordinal = pathItemParams.Ordinal,
                IsPausing = IsPausing
            };

            var pathItemJobs = new List<PathItemJobModel>();

            try
            {
                var providers = (await _queryService.GetSubmittingProviders(pathItemParams.CollectionName)).ToList();

                foreach (var provider in providers)
                {
                    var jobId = await _jobManager.AddJob(
                        await _jobFactory.CreateJobAsync(
                            GetYearSpecificCollectionName(pathItemParams.CollectionYear),
                            pathItemParams.CollectionYear,
                            pathItemParams.Period,
                            provider.UkPrn));
                    jobIds.Add(jobId);

                    pathItemJobs.Add(new PathItemJobModel
                    {
                        JobId = jobId,
                        Status = Convert.ToInt32(JobStatusType.Ready)
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            pathItem.PathItemJobs = pathItemJobs;

            pathItem.HasJobs = pathItemJobs.Any();

            await _stateService.SavePathItem(pathItem, pathItemParams.CollectionYear, pathItemParams.Period);

            return _returnFactory.CreatePathTaskReturn(pathItemJobs.Any(), jobIds.Any() ? jobIds : null, null);
        }

        private string GetYearSpecificCollectionName(int collectionYear)
        {
            return CollectionName.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString());
        }

        private string GetValidityKey()
        {
            return $"{PathItemId}-{EntityType}";
        }
    }
}