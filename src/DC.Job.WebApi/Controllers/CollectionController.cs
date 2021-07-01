using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/collections")]
    public class CollectionController : Controller
    {
        private readonly IOrganisationService _organisationService;
        private readonly ICollectionService _collectionService;

        public CollectionController(
            IOrganisationService organisationService,
            ICollectionService collectionService)
        {
            _organisationService = organisationService;
            _collectionService = collectionService;
        }

        // GET api/values/5
        [HttpGet("{ukprn}/{collectionType}")]
        public async Task<IEnumerable<Collection>> Get(long ukprn, string collectionType)
        {
            return await _organisationService.GetAvailableCollectionsAsync(ukprn, collectionType);
        }

        // GET api/values/5
        [HttpGet("all/{ukprn}")]
        public async Task<IEnumerable<Collection>> Get(long ukprn)
        {
            return await _organisationService.GetAvailableCollectionsAsync(ukprn);
        }

        // GET api/values/5
        [HttpGet("{collectionType}")]
        public async Task<Collection> Get(CancellationToken cancellationToken, string collectionType)
        {
            return await _collectionService.GetCollectionAsync(cancellationToken, collectionType);
        }

        [HttpGet]
        public async Task<IEnumerable<Collection>> Get(CancellationToken cancellationToken)
        {
            return await _collectionService.GetAllCollectionTypesAsync(cancellationToken);
        }

        [HttpGet("for-year/{collectionYear}")]
        public async Task<IEnumerable<Collection>> GetAsync(CancellationToken cancellationToken, int collectionYear)
        {
            return await _collectionService.GetCollectionsByYearAsync(cancellationToken, collectionYear);
        }

        [HttpGet("all-for-year/{collectionYear}")]
        public async Task<IEnumerable<Collection>> GetAllForYearAsync(CancellationToken cancellationToken, int collectionYear)
        {
            return await _collectionService.GetAllCollectionsByYearAsync(cancellationToken, collectionYear);
        }

        [HttpGet("available-years")]
        public async Task<IEnumerable<int>> GetAvailableYearsAsync(CancellationToken cancellationToken)
        {
            return await _collectionService.GetAvailableCollectionYearsAsync(cancellationToken);
        }

        [HttpGet("years/{collectionType}")]
        public async Task<IEnumerable<int>> GetCollectionYearsByTypeAsync(CancellationToken cancellationToken, string collectionType)
        {
            return await _collectionService.GetCollectionYearsByTypeAsync(cancellationToken, collectionType);
        }

        [HttpGet("type/{collectionType}")]
        public async Task<IEnumerable<Collection>> GetCollectionsByTypeAsync(string collectionType, CancellationToken cancellationToken)
        {
            return await _collectionService.GetCollectionsByTypeAsync(collectionType, cancellationToken);
        }

        [HttpGet("byId/{id}")]
        public async Task<Collection> GetByIdAsync(CancellationToken cancellationToken, int id)
        {
            return await _collectionService.GetCollectionAsync(cancellationToken, id);
        }

        [HttpGet("name/{collectionName}")]
        public async Task<Collection> GetCollectionFromNameAsync(CancellationToken cancellationToken, string collectionName)
        {
            return await _collectionService.GetCollectionFromNameAsync(cancellationToken, collectionName);
        }

        [HttpGet("byDate/{collectionType}")]
        public async Task<Collection> GetCollectionByDateAsync(CancellationToken cancellationToken, string collectionType, DateTime date)
        {
            return await _collectionService.GetCollectionByDateAsync(cancellationToken, collectionType, date);
        }

        [HttpGet("OpenByDateRange/{startDateUtc}/{endDateUtc}")]
        public async Task<IEnumerable<Collection>> GetCollectionsByDateRangeAsync(CancellationToken cancellationToken, DateTime startDateUtc, DateTime endDateUtc)
        {
            return await _collectionService.GetOpenCollectionsByDateRangeAsync(startDateUtc, endDateUtc, cancellationToken);
        }

        [HttpGet("collection-years/collection-type/{collectionType}/datetime/{dateTimeUtc}")]
        public async Task<IEnumerable<int>> GetApplicableCollectionYearsByTypeAsync(string collectionType, DateTime dateTimeUtc, CancellationToken cancellationToken)
        {
            return await _collectionService.GetApplicableCollectionYearsByTypeAsync(collectionType, dateTimeUtc, cancellationToken);
        }

        [HttpGet("name")]
        public async Task<List<Collection>> GetCollectionsFromNamesAsync(CancellationToken cancellationToken, [FromQuery] List<string> collectionNames)
        {
            return await _collectionService.GetCollectionsFromNamesAsync(cancellationToken, collectionNames);
        }

        [HttpGet("collectionStartDate/{collectionName}")]
        public async Task<DateTime?> GetCollectionStartDateAsync(string collectionName, CancellationToken cancellationToken)
        {
            return await _collectionService.GetCollectionStartDateAsync(collectionName, cancellationToken);
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("set-collection-processing-override/{collectionId}/{collectionProcessingOverrideStatus?}")]
        public async Task<bool> SetCollectionProcessingOverrideAsync(CancellationToken cancellationToken, int collectionId, bool? collectionProcessingOverrideStatus)
        {
            return await _collectionService.UpdateCollectionProcessingOverrideStatusAsync(cancellationToken, collectionId, collectionProcessingOverrideStatus);
        }

        [HttpGet("{collectionName}/related-links")]
        public async Task<IEnumerable<CollectionRelatedLink>> GetRelatedLinksAsync(CancellationToken cancellationToken, string collectionName)
        {
            return await _collectionService.GetRelatedLinksAsync(cancellationToken, collectionName);
        }

        [HttpGet("isOpenWithVariance/{id}/{now}/{negativeVarianceInMonths}/{positiveVarianceInMonths}")]
        public async Task<bool> IsCollectionOpenByIdWithVarianceAsync(int id, DateTime now, int negativeVarianceInMonths, int positiveVarianceInMonths, CancellationToken cancellationToken)
        {
            return await _collectionService.IsCollectionOpenByIdWithVarianceAsync(id, now, negativeVarianceInMonths, positiveVarianceInMonths, cancellationToken);
        }

        [HttpGet("countOfProviders/{collectionId}")]
        public async Task<int> GetCountOfProvidersForCollectionAsync(int collectionId, CancellationToken cancellationToken)
        {
            return await _collectionService.GetCountOfProvidersForCollectionAsync(collectionId, cancellationToken);
        }
    }
}