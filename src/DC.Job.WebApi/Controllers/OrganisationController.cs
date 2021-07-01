using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/org")]
    public class OrganisationController : Controller
    {
        private const int PimsSearchCount = 100;
        private readonly IOrganisationService _organisationService;
        private readonly IFcsContext _fcsContext;
        private readonly IJobQueryService _jobQueryService;
        private readonly ILogger _logger;
        private readonly IAuditFactory _auditFactory;
        private readonly IReportsArchiveService _reportsArchiveService;

        public OrganisationController(
            IOrganisationService organisationService,
            IFcsContext fcsContext,
            IJobQueryService jobQueryService,
            ILogger logger,
            IAuditFactory auditFactory,
            IReportsArchiveService reportsArchiveService)
        {
            _organisationService = organisationService;
            _fcsContext = fcsContext;
            _jobQueryService = jobQueryService;
            _logger = logger;
            _auditFactory = auditFactory;
            _reportsArchiveService = reportsArchiveService;
        }

        // GET api/values/5
        [HttpGet("collection-types/{ukprn}")]
        public async Task<IActionResult> GetAsync(long ukprn)
        {
            if (ukprn == 0)
            {
                return null;
            }

            try
            {
                return Ok(await _organisationService.GetAvailableCollectionTypesAsync(ukprn));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("collections/{ukprn}/{collectionName}")]
        public async Task<Collection> GetAsync(long ukprn, string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                return null;
            }

            return await _organisationService.GetCollectionAsync(ukprn, collectionName);
        }

        [HttpGet("attributes/{ukprn}")]
        public async Task<OrganisationAttributes> GetOrganisationAttributesAsync(long ukprn)
        {
            if (ukprn == 0)
            {
                return null;
            }

            return await _organisationService.GetOrganisationAttributes(ukprn);
        }

        // GET api/values/5
        [HttpGet("{ukprn}/{onlyActive?}")]
        public async Task<ProviderDetail> GetProviderAsync(CancellationToken cancellationToken, long ukprn, bool onlyActive = false)
        {
            if (ukprn == 0)
            {
                return null;
            }

            return await _organisationService.GetProviderAsync(cancellationToken, ukprn, onlyActive);
        }

        [HttpPost("pims")]
        public async Task<List<Provider>> GetAllValidPimsProvidersAsync([FromBody] IEnumerable<long> providerUkprns)
        {
            return await _organisationService.GetAllValidPimsProviders(providerUkprns);
        }

        [HttpPost("collections/collection-type/{collectionType}")]
        public async Task<List<OrganisationCollection>> GetOrgCollectionsByTypeAsync([FromBody] IEnumerable<long> providerUkprns, string collectionType, CancellationToken cancellationToken)
        {
            return await _organisationService.GetOrgCollectionsByTypeAsync(providerUkprns, collectionType, cancellationToken);
        }

        [HttpPost("pimsactive")]
        public async Task<List<Provider>> GetAllActiveValidPimsProvidersAsync([FromBody] IEnumerable<long> providerUkprns)
        {
            return await _organisationService.GetAllValidAndActivePimsProviders(providerUkprns);
        }

        [HttpGet("assignments/{ukprn}")]
        public async Task<IEnumerable<OrganisationCollection>> GetProviderAssignmentsAsync(long ukprn)
        {
            return await _organisationService.GetProviderAssignments(ukprn);
        }

        [HttpGet("assignments/{ukprn}/{collectionType}")]
        public async Task<IEnumerable<OrganisationCollection>> GetProviderAssignmentsAsync(long ukprn, string collectionType)
        {
            return await _organisationService.GetProviderAssignments(ukprn, collectionType);
        }

        [HttpPost("all-assignments")]
        public async Task<IEnumerable<OrganisationCollection>> GetAllProviderAssignmentsAsync([FromBody] IEnumerable<long> listOfProviderUkprns)
        {
            return await _organisationService.GetAllProviderAssignmentsAsync(listOfProviderUkprns);
        }

        // GET api/values/5
        [HttpGet("esf/contracts/{ukprn}")]
        public async Task<int> GetEsfContractsAsync(long ukprn)
        {
            if (ukprn == 0)
            {
                return 0;
            }

            return await _fcsContext.ContractAllocations.CountAsync(x => x.Contract.Contractor.Ukprn == ukprn);
        }

        // GET api/values/5
        [HttpGet("search/{searchTerm}/{count?}")]
        public async Task<IEnumerable<ProviderDetail>> SearchProvidersAsync(CancellationToken cancellationToken, string searchTerm, int count = 25)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return null;
            }

            var result = new List<ProviderDetail>();

            List<Provider> providersInPimsList = await _organisationService.SearchProvidersInPimsAsync(searchTerm, PimsSearchCount, false);

            if (providersInPimsList.Any())
            {
                var ukprns = providersInPimsList.Select(x => x.Ukprn).ToList();

                // get providers who ever have/had a collection assignment
                var activeProviders = await _organisationService.GetProvidersWithCollectionAssignmentAsync(ukprns);

                var archiveProviders = await _reportsArchiveService.GetProvidersWithDataAsync(cancellationToken, ukprns);

                // get all the last jobs for providers
                var lastJobs = (await _jobQueryService.GetLatestJobByUkprnAsync(activeProviders.ToArray(), CancellationToken.None)).ToList();

                // get which active providers have a Funding Claims allocation
                var fundingClaims = (await _organisationService.GetProvidersWithFundingClaims(activeProviders)).ToHashSet();

                //combine the list of active providers and any who has reports archive data
                var resultProviders = archiveProviders.Union(activeProviders);

                foreach (var ukprn in resultProviders)
                {
                    var org = providersInPimsList.Where(x => x.Ukprn == ukprn)
                        .OrderByDescending(x => x.StatusOrder)
                        .FirstOrDefault();
                    var provider = new ProviderDetail
                    {
                        Ukprn = org.Ukprn,
                        Name = org.Name,
                        HasFundingClaims = fundingClaims.Contains(org.Ukprn),
                        ProviderLatestSubmissions = lastJobs.Where(x => x.Ukprn == org.Ukprn),
                    };

                    result.Add(provider);
                }
            }

            return result;
        }

        [HttpGet("search/new/{searchTerm}/{count?}")]
        public async Task<ProviderSearchResult> SearchNewProvidersAsync(string searchTerm, int count = 25, CancellationToken cancellationToken = default(CancellationToken))
        {
            var providerSearchResult = new ProviderSearchResult();
            var providersInPimsList = await _organisationService.SearchProvidersInPimsAsync(searchTerm, PimsSearchCount);
            var ukprnsInJobMgmt = await _organisationService.FilterProvidersInJobMgmtAsync(providersInPimsList.Select(x => x.Ukprn).ToList(), cancellationToken);

            providerSearchResult.Providers = providersInPimsList.Where(x => !ukprnsInJobMgmt.Contains(x.Ukprn)).Take(count).ToList();
            return providerSearchResult;
        }

        [HttpGet("search/existing/{searchTerm}/{count?}")]
        public async Task<ProviderSearchResult> SearchExistingProvidersAsync(string searchTerm, int count = 25, CancellationToken cancellationToken = default(CancellationToken))
        {
            var providerSearchResult = new ProviderSearchResult();

            var providersInPimsList = await _organisationService.SearchProvidersInPimsAsync(searchTerm, PimsSearchCount, false); // Get all matching from PIMS, including inactive ones
            var pimsUkprnAndActives = providersInPimsList.Select(x => new UkprnAndActive { Ukprn = x.Ukprn, IsActive = x.ActiveInPIMS }).ToList();

            var providerStatus = await _organisationService.GetProviderStatusInJobMgmtAsync(pimsUkprnAndActives, cancellationToken);

            var providerStatusDict = providerStatus.ToDictionary(ps => ps.Ukprn, ps => ps.ProviderStatus);
            foreach (var providerInPimsList in providersInPimsList)
            {
                if (providerStatusDict.TryGetValue(providerInPimsList.Ukprn, out var provStatus))
                {
                    providerInPimsList.ProviderStatus = provStatus;
                    providerInPimsList.ExistsInSld = provStatus != ProviderStatusType.Available &&
                                                     provStatus != ProviderStatusType.PimsInactiveOnly;
                }
            }

            providerSearchResult.Providers = providersInPimsList.Where(p => p.ProviderStatus != ProviderStatusType.PimsInactiveOnly).Take(count);
            return providerSearchResult;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("add")]
        public async Task<IActionResult> AddOrganisation([FromBody] Organisation organisation, CancellationToken cancellationToken)
        {
            try
            {
                if (await _organisationService.AddOrganisation(organisation, cancellationToken))
                {
                    return Ok();
                }

                return StatusCode((int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding new organisation. UKPRN:{organisation.Ukprn}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("addbulk")]
        public async Task<IActionResult> AddBulkOrganisationsAsync([FromBody] List<Organisation> organisations)
        {
            try
            {
                await _organisationService.AddBulkOrganisationsAsync(organisations);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding new organisations.", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("addbulk-collections")]
        public async Task<IActionResult> AddBulkOrganisationCollectionsAsync([FromBody] List<OrganisationCollection> organisationCollections)
        {
            try
            {
                await _organisationService.AddBulkOrganisationCollectionsAsync(organisationCollections);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding new OrganisationCollections.", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrganisation([FromBody] Organisation organisation, CancellationToken cancellationToken)
        {
            try
            {
                if (await _organisationService.UpdateOrganisation(organisation, cancellationToken))
                {
                    return Ok();
                }

                var returnAction = CreatedAtAction("UpdateOrganisationAsync", new { ukprn = organisation.Ukprn }, organisation);
                returnAction.StatusCode = StatusCodes.Status204NoContent;
                return returnAction;
            }
            catch (Exception ex)
            {
                var returnAction = CreatedAtAction("UpdateOrganisationAsync", new { ukprn = organisation.Ukprn }, organisation);
                returnAction.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError($"Error updating organisation. UKPRN:{organisation.Ukprn}", ex);
                return returnAction;
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("assignments/update/{ukprn}")]
        public async Task<IActionResult> UpdateAssignmentsAsync(long ukprn, [FromBody] IEnumerable<OrganisationCollection> organisationCollections)
        {
            if (await _organisationService.UpdateAssignments(ukprn, organisationCollections))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("assignments/delete/{ukprn}")]
        public async Task<IActionResult> DeleteAssignmentsAsync(long ukprn, [FromBody] IEnumerable<OrganisationCollection> organisationCollections)
        {
            if (await _organisationService.DeleteAssignments(ukprn, organisationCollections))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
