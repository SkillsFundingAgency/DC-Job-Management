using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.Providers;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PIMS.EF;
using ESFA.DC.PIMS.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ESFA.DC.JobQueueManager
{
    public class OrganisationService : IOrganisationService
    {
        private const int ACTIVE = 1;
        private const int ISNUMERIC = 1;
        private const int UPINLENGTH = 6;
        private const int MAIN_CORRESPONDENCE_ADDRESSS = 1;

        private const int ACTIVECODE = 214;
        private const int LOCKEDRESTRUCTURECODE = 216;
        private const int PROVIDERUNDERCHANGECODE = 392;
        private List<decimal> OrgValidationStatusCodes = new List<decimal> { ACTIVECODE, LOCKEDRESTRUCTURECODE, PROVIDERUNDERCHANGECODE };

        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly Func<IPimsContext> _pimsContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly ILogger _logger;
        private readonly IAuditFactory _auditFactory;

        public OrganisationService(
            Func<IJobQueueDataContext> contextFactory,
            Func<IPimsContext> pimsContextFactory,
            IDateTimeProvider dateTimeProvider,
            IReturnCalendarService returnCalendarService,
            ILogger logger,
            IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _pimsContextFactory = pimsContextFactory;
            _dateTimeProvider = dateTimeProvider;
            _returnCalendarService = returnCalendarService;
            _logger = logger;
            _auditFactory = auditFactory;
        }

        /// <summary>
        /// Returns the collection types available to the provider (taking into account enabled status and date).
        /// Used by GUI to build the radio buttons.
        /// </summary>
        /// <param name="ukprn">The provider.</param>
        /// <returns>Th collections types available to the user.</returns>
        public async Task<IEnumerable<CollectionType>> GetAvailableCollectionTypesAsync(long ukprn)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var data = await context.OrganisationCollection
                    .Where(x => x.Organisation.Ukprn == ukprn &&
                                (((x.StartDateTimeUtc == null || x.StartDateTimeUtc < dateTimeNowUtc) &&
                                (x.EndDateTimeUtc == null || x.EndDateTimeUtc > dateTimeNowUtc))
                                || x.Collection.CollectionType.Type == CollectionTypeConstants.DevolvedContracts))
                    .GroupBy(x => x.Collection.CollectionType)
                    .ToListAsync();
                var items = data.Select(y => new CollectionType()
                {
                    Description = y.Key.Description,
                    Type = y.Key.Type
                });

                return items;
            }
        }

        public async Task<List<long>> GetProvidersWithCollectionAssignmentAsync(List<long> ukprns = null)
        {
            using (var context = _contextFactory())
            {
                var data = await context.Organisation
                    .Where(x => (ukprns == null || ukprns.Contains(x.Ukprn)) && (x.IsMca || x.OrganisationCollection.Any()))
                    .Select(x => x.Ukprn)
                    .ToListAsync();

                return data;
            }
        }

        /// <summary>
        /// Provides the available collection type (ILR1819/ILR1920) for a provider and collection.
        /// </summary>
        /// <param name="ukprn">The provider.</param>
        /// <param name="collectionType">The collection type.</param>
        /// <returns>The available collections.</returns>
        public async Task<IEnumerable<Collection>> GetAvailableCollectionsAsync(long ukprn, string collectionType = null)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var items = await context.OrganisationCollection
                    .Where(x => x.Organisation.Ukprn == ukprn && x.Collection.IsOpen &&
                                (((x.StartDateTimeUtc == null || x.StartDateTimeUtc < dateTimeNowUtc) &&
                                (x.EndDateTimeUtc == null || x.EndDateTimeUtc > dateTimeNowUtc) &&
                                (string.IsNullOrEmpty(collectionType) || x.Collection.CollectionType.Type == collectionType))
                                || (x.Collection.CollectionType.Type == CollectionTypeConstants.DevolvedContracts && (string.IsNullOrEmpty(collectionType) || collectionType == CollectionTypeConstants.DevolvedContracts))))
                    .Select(y => new Collection()
                    {
                        CollectionTitle = y.Collection.Name,
                        IsOpen = y.Collection.IsOpen,
                        CollectionType = y.Collection.CollectionType.Type,
                        CollectionYear = y.Collection.CollectionYear.GetValueOrDefault(),
                        Description = y.Collection.Description,
                        SubText = y.Collection.SubText,
                        StorageReference = y.Collection.StorageReference,
                        CollectionId = y.CollectionId,
                        FileNameRegex = y.Collection.FileNameRegex
                    })
                    .ToListAsync();
                foreach (var collection in items)
                {
                    collection.OpenPeriodNumber = (await _returnCalendarService.GetCurrentPeriodAsync(collection.CollectionTitle))?.PeriodNumber;

                    if (!collection.OpenPeriodNumber.HasValue)
                    {
                        var nextPeriod = await _returnCalendarService.GetNextPeriodAsync(collection.CollectionTitle);
                        collection.NextPeriodNumber = nextPeriod?.PeriodNumber;
                        collection.NextPeriodOpenDateTimeUtc = nextPeriod?.StartDateTimeUtc;
                    }
                }

                return items;
            }
        }

        /// <summary>
        /// Get's the specified collection for the provider taking into account whether the provider is enabled.
        /// </summary>
        /// <param name="ukprn">The provider.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The available collection, or null.</returns>
        public async Task<Collection> GetCollectionAsync(long ukprn, string collectionName)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var data = await context.OrganisationCollection
                    .Include(x => x.Collection)
                    .ThenInclude(x => x.CollectionType)
                    .Where(x => x.Organisation.Ukprn == ukprn &&
                                (x.StartDateTimeUtc == null || x.StartDateTimeUtc < dateTimeNowUtc) &&
                                (x.EndDateTimeUtc == null || x.EndDateTimeUtc > dateTimeNowUtc) &&
                                x.Collection.Name.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefaultAsync();
                if (data != null)
                {
                    return new Collection()
                    {
                        CollectionTitle = data.Collection.Name,
                        IsOpen = data.Collection.IsOpen,
                        CollectionType = data.Collection.CollectionType.Type,
                        CollectionYear = data.Collection.CollectionYear.GetValueOrDefault(),
                        Description = data.Collection.Description,
                        SubText = data.Collection.SubText,
                        StorageReference = data.Collection.StorageReference,
                        CollectionId = data.CollectionId,
                        FileNameRegex = data.Collection.FileNameRegex
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Get the attributes related to provider.
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        public async Task<OrganisationAttributes> GetOrganisationAttributes(long ukprn)
        {
            using (var context = _contextFactory())
            {
                var data = await context.Organisation.SingleOrDefaultAsync(x => x.Ukprn == ukprn);

                if (data != null)
                {
                    return new OrganisationAttributes()
                    {
                        Ukprn = ukprn,
                        IsMca = data.IsMca
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Get Organisation By UKPRN.
        /// </summary>
        /// <param name="ukprn">UKPRN.</param>
        /// <returns>Organisation.</returns>
        public async Task<Organisation> GetByUkprn(long ukprn)
        {
            using (var context = _contextFactory())
            {
                var data = await context.Organisation.SingleOrDefaultAsync(x => x.Ukprn == ukprn);

                if (data != null)
                {
                    return new Organisation()
                    {
                        Ukprn = ukprn,
                        IsMca = data.IsMca
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Add new Organisation.
        /// </summary>
        /// <param name="organisation">Organisation</param>
        /// <returns>A <see cref="Task"/> Boolean value representing the success of the operation</returns>
        public async Task<bool> AddOrganisation(Organisation organisation, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await BuildProviderFunc(organisation), context);

                var org = context.Organisation.SingleOrDefault(o => o.Ukprn == organisation.Ukprn);

                if (org == null)
                {
                    context.Organisation.Add(new Data.Entities.Organisation()
                    {
                        Ukprn = organisation.Ukprn,
                        IsMca = organisation.IsMca
                    });
                }
                else
                {
                    return false;
                }

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(cancellationToken);

                return true;
            }
        }

        /// <summary>
        /// Updates an existing Organisation.
        /// </summary>
        /// <param name="organisation">Organisation.</param>
        /// <returns>A <see cref="Task"/> Bool representing the success of the operation.</returns>
        public async Task<bool> UpdateOrganisation(Organisation organisation, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await BuildProviderFunc(organisation), context);
                await audit.BeforeAsync(cancellationToken);

                var org = context.Organisation.SingleOrDefault(o => o.Ukprn == organisation.Ukprn);

                if (org == null)
                {
                    return false;
                }
                else
                {
                    org.IsMca = organisation.IsMca;
                }

                await context.SaveChangesAsync();

                await audit.AfterAndSaveAsync(cancellationToken);

                return true;
            }
        }

        /// <summary>
        /// Gets a collection of Provider Assignments.
        /// </summary>
        /// <param name="ukprn">UKPRN.</param>
        /// <param name="collectionType">collectionType.</param>
        /// <returns>Provider Assignments.</returns>
        public async Task<IEnumerable<OrganisationCollection>> GetProviderAssignments(long ukprn, string collectionType = null)
        {
            using (var context = _contextFactory())
            {
                return await context.OrganisationCollection
                    .Include(o => o.Organisation)
                    .Include(c => c.Collection)
                    .Where(org => org.Organisation.Ukprn == ukprn && (string.IsNullOrEmpty(collectionType) || org.Collection.CollectionType.Type == collectionType))
                    .Select(s =>
                        new OrganisationCollection(
                            s.Ukprn.Value,
                            s.CollectionId,
                            s.Collection.CollectionTypeId,
                            s.Collection.CollectionType.Type,
                            s.Collection.Name,
                            s.StartDateTimeUtc.GetValueOrDefault(),
                            s.EndDateTimeUtc))
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Return all collection assignments for a given list of provider ukprns.
        /// </summary>
        /// <param name="listOfProviderUkprns">provided list of provider ukprns.</param>
        /// <returns>collection assignments relating to list of provided ukprns.</returns>
        public async Task<IEnumerable<OrganisationCollection>> GetAllProviderAssignmentsAsync(IEnumerable<long> listOfProviderUkprns)
        {
            using (var context = _contextFactory())
            {
                return await context.OrganisationCollection
                    .Include(c => c.Collection)
                    .ThenInclude(t => t.CollectionType)
                    .Where(col => listOfProviderUkprns.Contains(col.Ukprn.Value))
                    .Select(s => new OrganisationCollection(
                        s.Ukprn.Value,
                        s.CollectionId,
                        s.Collection.CollectionTypeId,
                        s.Collection.CollectionType.Type,
                        s.Collection.Name,
                        s.StartDateTimeUtc.GetValueOrDefault(),
                        s.EndDateTimeUtc))
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateAssignments(long ukprn, IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Entered UpdateAssignments - Job Management. Number of organisationsCollections to update: {organisationCollections.Count()}");
            try
            {
                using (var context = _contextFactory())
                {
                    var organisation = await context.Organisation.SingleAsync(o => o.Ukprn == ukprn, cancellationToken);
                    _logger.LogInfo($"Entered UpdateAssignments - Job Management. Organisation: {ukprn} retrieved");
                    var auditList = new List<IAudit>();
                    foreach (var organisationCollection in organisationCollections)
                    {
                        var audit = _auditFactory.BuildDataAudit(await BuildAssignmentsFunc(organisationCollection, ukprn), context);
                        await audit.BeforeAsync(cancellationToken);
                        auditList.Add(audit);

                        var collection = await context.Collection.SingleAsync(s => s.CollectionId == organisationCollection.CollectionId, cancellationToken);
                        _logger.LogInfo($"Entered UpdateAssignments - Job Management. Collection: {organisationCollection.CollectionId} retrieved");

                        var orgCollection = await context.OrganisationCollection.SingleOrDefaultAsync(x => x.Organisation == organisation && x.Collection == collection, cancellationToken);
                        _logger.LogInfo($"Entered UpdateAssignments - Job Management. OrgCollection retrieved");

                        if (orgCollection != null)
                        {
                            orgCollection.StartDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(organisationCollection.StartDate);
                            orgCollection.EndDateTimeUtc = organisationCollection.EndDate.HasValue ? (DateTime?)_dateTimeProvider.ConvertUkToUtc(organisationCollection.EndDate.Value) : null;
                        }
                        else
                        {
                            await context.OrganisationCollection.AddAsync(
                                new Data.Entities.OrganisationCollection()
                                {
                                    Organisation = organisation,
                                    Collection = collection,
                                    Ukprn = ukprn,
                                    CollectionId = organisationCollection.CollectionId,
                                    OrganisationId = organisation.OrganisationId,
                                    StartDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(organisationCollection.StartDate),
                                    EndDateTimeUtc = organisationCollection.EndDate.HasValue ? (DateTime?)_dateTimeProvider.ConvertUkToUtc(organisationCollection.EndDate.Value) : null,
                                }, cancellationToken);
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    foreach (var auditedItem in auditList)
                    {
                        await auditedItem.AfterAndSaveAsync(cancellationToken);
                    }

                    _logger.LogInfo($"Entered UpdateAssignments - Job Management. Successfully update record.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Saving Organisation Assignments", ex);
                return false;
            }
        }

        public async Task<bool> DeleteAssignments(long ukprn, IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var organisation = await context.Organisation.SingleAsync(o => o.Ukprn == ukprn, cancellationToken);
                    var auditList = new List<IAudit>();

                    foreach (var organisationCollection in organisationCollections)
                    {
                        var audit = _auditFactory.BuildDataAudit(await BuildAssignmentsFunc(organisationCollection, ukprn), context);
                        await audit.BeforeAsync(cancellationToken);
                        auditList.Add(audit);
                        var collection = await context.Collection.SingleAsync(s => s.CollectionId == organisationCollection.CollectionId, cancellationToken);

                        var orgCollection = await context.OrganisationCollection.SingleOrDefaultAsync(x => x.Organisation == organisation && x.Collection == collection, cancellationToken);

                        if (orgCollection != null)
                        {
                            context.OrganisationCollection.Remove(orgCollection);
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    foreach (var auditedItem in auditList)
                    {
                        await auditedItem.AfterAndSaveAsync(cancellationToken);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Deleting Organisation Assignments", ex);
                return false;
            }
        }

        public async Task<IList<Provider>> GetProviderStatusInJobMgmtAsync(List<UkprnAndActive> pimsUkprnAndActives, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                try
                {
                    var ukprnJson = JsonConvert.SerializeObject(pimsUkprnAndActives);

                    var result = await context.FromSqlAsync<Provider>(
                        CommandType.StoredProcedure,
                        "GetProviderStatus",
                        new { ukprns = ukprnJson });
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Deleting Organisation Assignments", ex);
                    return null;
                }
            }
        }

        public async Task<List<Provider>> SearchProvidersInPimsAsync(string searchTerm, int count, bool includeActiveOnly = true)
        {
            // This SQL is built in code rather than an SP as we don't have write access to the PIMS Db to add SP's etc.

            var isActiveCheck = $"Org.STATUS_ID = {ACTIVE} AND OrgCode.ORG_CODE_VAL_ID IN ({ACTIVECODE}, {LOCKEDRESTRUCTURECODE}, {PROVIDERUNDERCHANGECODE})";

            var searchSection = $@" ISNUMERIC(ORG_CODE) = {ISNUMERIC} AND
	                    LEN(ORG_CODE) = {UPINLENGTH} AND
	                    ((CHARINDEX(@searchTerm, Org.ORG_NAME) > 0) OR
	                    (CHARINDEX(@searchTerm, Org.ORG_TRADING_NAME) > 0) OR
	                    (CHARINDEX(@searchTerm, CONVERT(VARCHAR(100), OrgUkprn.UKPRN)) > 0) OR
                        (CHARINDEX(@searchTerm, Org.ORG_CODE) > 0)";

            string searchTermLimited = string.Empty;
            if (searchTerm.Contains(" ltd") || searchTerm.Contains(" limited"))
            {
                if (searchTerm.Contains(" ltd"))
                {
                    searchTermLimited = searchTerm.Replace(" ltd", " limited");
                }
                else if (searchTerm.Contains(" limited"))
                {
                    searchTermLimited = searchTerm.Replace(" limited", " ltd");
                }

                searchSection += $" OR ((CHARINDEX(@searchTermLimited, Org.ORG_NAME) > 0) OR (CHARINDEX(@searchTermLimited, Org.ORG_TRADING_NAME) > 0)) ";
            }

            searchSection += ")";

            if (includeActiveOnly)
            {
                searchSection = $"({isActiveCheck}) AND" + searchSection;
            }

            var selectSection = $@"SELECT TOP({count})
	                        ORG_NAME AS Name,
	                        ORG_TRADING_NAME AS TradingName,
	                        Org.ORG_CODE AS Upin,
	                        CAST(OrgUkprn.UKPRN AS bigint) AS Ukprn,
                            MAX(CASE WHEN {isActiveCheck} THEN 1 ELSE 0 END) as ActiveInPIMS,
                            CASE IsNull(Org.STATUS_ID,-1) WHEN 1 THEN 99 ELSE IsNull(Org.STATUS_ID,-1) END AS StatusOrder
                        FROM
	                        ORGS AS Org
	                        INNER JOIN ORG_UKPRNS AS OrgUkprn ON Org.ORG_ID = OrgUkprn.ORG_ID
							INNER JOIN ORG_CODES AS OrgCode ON Org.ORG_ID = OrgCode.ORG_ID
                        WHERE ";

            var mainSearch = $"{selectSection} {searchSection} GROUP BY ORG_NAME, ORG_TRADING_NAME, Org.ORG_CODE, OrgUkprn.UKPRN, Org.STATUS_ID";

            var rankedSearch = "SELECT *, RANK () OVER (PARTITION BY Ukprn ORDER BY Ukprn, ActiveInPIMS DESC, StatusOrder DESC, Upin DESC) AS Rank FROM MatchingEntries m";

            var sql = $@"WITH MatchingEntries AS ({mainSearch}),
                             RankedEntries as ({rankedSearch})
                        SELECT * FROM RankedEntries WHERE Rank = 1 ORDER BY Name;";

            var pimsProviderList = new List<Provider>();

            try
            {
                using (var pimsContext = _pimsContextFactory())
                {
                    pimsProviderList = (await pimsContext.FromSqlAsync<Provider>(
                        CommandType.Text,
                        sql,
                        new
                        {
                            searchTerm,
                            searchTermLimited
                        })).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Searching Providers In PIMS", ex);
            }

            return pimsProviderList;
        }

        public async Task<List<Provider>> GetAllValidPimsProviders(IEnumerable<long> providerUkrpns, CancellationToken cancellationToken)
        {
            using (var orgContext = _pimsContextFactory())
            {
                var data = await orgContext.Orgs
                    .Include(i => i.OrgUkprn)
                    .Where(x => providerUkrpns.Contains((long)x.OrgUkprn.Ukprn) && x.StatusId == 1)
                    .ToListAsync(cancellationToken);

                return data.Where(d => int.TryParse(d.OrgCode, out _) && d.OrgCode.Length == 6)
                    .Select(x => new Provider() { Ukprn = (long)x.OrgUkprn.Ukprn, Name = x.OrgName })
                    .ToList();
            }
        }

        public async Task<List<Provider>> GetAllValidAndActivePimsProviders(IEnumerable<long> providerUkrpns, CancellationToken cancellationToken)
        {
            using (var orgContext = _pimsContextFactory())
            {
                var data = await orgContext.Orgs
                    .Include(i => i.OrgUkprn)
                    .Include(i => i.OrgCodes)
                    .Where(x => providerUkrpns.Contains((long)x.OrgUkprn.Ukprn) &&
                                x.StatusId == 1 &&
                                x.OrgCodes.Any(oc => oc.OrgCodeValId.HasValue && OrgValidationStatusCodes.Contains(oc.OrgCodeValId.Value)))
                    .ToListAsync(cancellationToken);

                return data.Where(d => int.TryParse(d.OrgCode, out _) && d.OrgCode.Length == 6)
                    .Select(x => new Provider() { Ukprn = (long)x.OrgUkprn.Ukprn, Name = x.OrgName })
                    .ToList();
            }
        }

        public async Task AddBulkOrganisationsAsync(IEnumerable<Organisation> organisations, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"{nameof(AddBulkOrganisationsAsync)} adding {organisations?.Count()} organisations");
            using (var context = _contextFactory())
            {
                var ukprns = organisations.Select(o => o.Ukprn).ToList();

                var result = await context.FromSqlAsync<int>(
                    CommandType.StoredProcedure,
                    "BulkAddProviders",
                    new { ukprns = JsonConvert.SerializeObject(ukprns) });
            }
        }

        public async Task AddBulkOrganisationCollectionsAsync(IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInfo($"{nameof(AddBulkOrganisationCollectionsAsync)} adding {organisationCollections?.Count()} organisation Collections");
            var orgCollections = organisationCollections.ToList();

            if (orgCollections.Any(oc => string.IsNullOrWhiteSpace(oc.CollectionName)))
            {
                throw new ArgumentException("Missing Collection Name");
            }

            if (orgCollections.Any(oc => oc.StartDate != (DateTime)SqlDateTime.MinValue && oc.EndDate.HasValue == true))
            {
                throw new ArgumentException("Start and End dates both specified");
            }

            var distinctUkprns = orgCollections.GroupBy(x => x.Ukprn).Select(x => x.Key).ToList();

            using (var context = _contextFactory())
            {
                var organisationsInDb = await context.Organisation.Where(o => distinctUkprns.Contains(o.Ukprn))
                    .ToListAsync(cancellationToken);
                var organisationUkprnDictionary = organisationsInDb.ToDictionary(o => o.Ukprn, o => o.OrganisationId);

                if (organisationsInDb.Count != distinctUkprns.Count)
                {
                    throw new ApplicationException("Missing Organisations");
                }

                var organisationCollectionsInDb = await context.OrganisationCollection
                    .Include(oc => oc.Organisation)
                    .Include(oc => oc.Collection)
                    .Where(o => distinctUkprns.Contains(o.Organisation.Ukprn)).
                    ToListAsync(cancellationToken);
                var organisationCollectionsUkPrnDictionary = organisationCollectionsInDb
                    .Where(oc => oc.Ukprn.HasValue)
                    .GroupBy(oc => oc.Ukprn)
                    .ToDictionary(ocg => ocg.Key);

                var collections = await context.Collection
                    .Include(c => c.CollectionType)
                    .Where(c => c.CollectionType.IsProviderAssignableInOperations).ToListAsync(cancellationToken);
                var collectionNameDictionary = collections.ToDictionary(c => c.Name, c => c.CollectionId);

                var orgCollsToSetEndDate = orgCollections.Where(oc =>
                {
                    if (oc.EndDate.HasValue &&
                        organisationCollectionsUkPrnDictionary.TryGetValue(oc.Ukprn, out var orgColls))
                    {
                        var orgColl = orgColls.FirstOrDefault(ocd => ocd.Collection?.Name == oc.CollectionName);
                        return orgColl.EndDateTimeUtc != oc.EndDate;
                    }

                    return false;
                }).ToList();

                var orgCollsToRemoveEndDate = orgCollections.Where(oc =>
                {
                    if (!oc.EndDate.HasValue && organisationCollectionsUkPrnDictionary.TryGetValue(oc.Ukprn, out var orgColls))
                    {
                        var orgColl = orgColls.FirstOrDefault(ocd => ocd.Collection?.Name == oc.CollectionName);
                        return orgColl?.EndDateTimeUtc != null;
                    }

                    return false;
                }).ToList();

                var orgCollsToAdd = orgCollections.Where(oc =>
                {
                    if (organisationCollectionsUkPrnDictionary.TryGetValue(oc.Ukprn, out var orgColls))
                    {
                        return orgColls.All(ocd => ocd.Collection?.Name != oc.CollectionName);
                    }

                    return true;
                }).ToList();

                if (orgCollsToSetEndDate.Any() || orgCollsToRemoveEndDate.Any() || orgCollsToAdd.Any())
                {
                    var dbOrgCollsToSetEndDate = SetupIds(orgCollsToSetEndDate, organisationUkprnDictionary, collectionNameDictionary);
                    var dbOrgCollsToRemoveEndDate = SetupIds(orgCollsToRemoveEndDate, organisationUkprnDictionary, collectionNameDictionary);
                    var dbOrgCollsToAdd = SetupIds(orgCollsToAdd, organisationUkprnDictionary, collectionNameDictionary);

                    _logger.LogInfo($"{nameof(AddBulkOrganisationCollectionsAsync)} calling BulkAddProviderCollections with {dbOrgCollsToSetEndDate?.Count()} dbOrgCollsToSetEndDate, {dbOrgCollsToRemoveEndDate?.Count} dbOrgCollsToRemoveEndDate, {dbOrgCollsToAdd?.Count} dbOrgCollsToAdd");

                    var result = await context.FromSqlAsync<int>(
                        CommandType.StoredProcedure,
                        "BulkAddProviderCollections",
                        new
                        {
                            setEndDate = JsonConvert.SerializeObject(dbOrgCollsToSetEndDate),
                            removeEndDate = JsonConvert.SerializeObject(dbOrgCollsToRemoveEndDate),
                            add = JsonConvert.SerializeObject(dbOrgCollsToAdd)
                        });
                }
            }
        }

        public async Task<IList<long>> FilterProvidersInJobMgmtAsync(IList<long> ukprns, CancellationToken cancellationToken)
        {
            var jobMgmtUkPrns = new List<long>();

            try
            {
                using (var context = _contextFactory())
                {
                    jobMgmtUkPrns = await context.Organisation.Where(x => ukprns.Contains(x.Ukprn)).Select(x => x.Ukprn).ToListAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Searching Providers in Job Management", ex);
            }

            return jobMgmtUkPrns;
        }

        public async Task<ProviderDetail> GetProviderAsync(CancellationToken cancellationToken, long ukprn, bool onlyActive = false)
        {
            Data.Entities.Organisation organisation;
            Org org;
            try
            {
                using (var context = _contextFactory())
                {
                    organisation = await context.Organisation.SingleOrDefaultAsync(x => x.Ukprn == ukprn, cancellationToken);
                }

                using (var pimsContext = _pimsContextFactory())
                {
                    var query = pimsContext.Orgs
                    .Include(i => i.OrgCodes)
                    .Where(x => x.OrgUkprn != null && (x.OrgUkprn.Ukprn == ukprn));
                    if (onlyActive)
                    {
                        query = query.Where(x => x.StatusId == ACTIVE && x.OrgCodes.Any(oc => OrgValidationStatusCodes.Contains(oc.OrgCodeValId.Value)));
                    }

                    org = await query.FirstOrDefaultAsync(cancellationToken);
                }

                if (org != null)
                {
                    var activeInPIMS = org.StatusId == ACTIVE &&
                                       org.OrgCodes.Any(oc => OrgValidationStatusCodes.Contains(oc.OrgCodeValId.Value));

                    var providerStatus = await GetProviderStatusInJobMgmtAsync(new List<UkprnAndActive> { new UkprnAndActive { Ukprn = ukprn, IsActive = activeInPIMS } }, cancellationToken);

                    return new ProviderDetail()
                    {
                        Ukprn = ukprn,
                        Name = org.OrgName,
                        Upin = org.OrgCode,
                        IsMCA = organisation?.IsMca ?? false,
                        ActiveInPIMS = activeInPIMS,
                        ProviderStatus = providerStatus.Single().ProviderStatus
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetProvider", ex);
            }

            return null;
        }

        public async Task<List<long>> GetProvidersWithFundingClaims(List<long> ukprns)
        {
            using (var context = _contextFactory())
            {
                return await context.OrganisationCollection
                    .Where(oc => oc.Ukprn.HasValue && ukprns.Contains(oc.Ukprn.Value) &&
                                 oc.Collection.CollectionType.Type == CollectionTypeConstants.FundingClaims)
                    .Select(oc => oc.Ukprn.Value)
                    .ToListAsync();
            }
        }

        public async Task<List<OrganisationCollection>> GetOrgCollectionsByTypeAsync(IEnumerable<long> ukprns, string collectionType, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Retrieving Org data.");

            using (var context = _contextFactory())
            {
                return await context
                    .OrganisationCollection
                    .Where(x =>
                        x.Collection.CollectionType.Type == collectionType
                        && ukprns.Contains(x.Ukprn.GetValueOrDefault()))
                    .Select(x => new OrganisationCollection
                    {
                        Ukprn = x.Ukprn.GetValueOrDefault(),
                        CollectionId = x.CollectionId
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<ProviderAddress> GetProviderAddressAsync(long ukprn)
        {
            var providerAddress = new ProviderAddress();
            var sql = $@"SELECT
	                        ORG_NAME AS Name,
                            addresses.ADDRESS_1 as Address1,
							addresses.ADDRESS_2 as Address2,
							ADDRESS_3 as Address3,
							ADDRESS_4 as Address4,
							ADDRESS_TOWN as Town,
							ADDRESS_POSTCODE as Postcode,
							ADDRESS_COUNTRY as Country
                        FROM
	                         ORGS AS Org
	                        INNER JOIN ORG_UKPRNS AS OrgUkprn ON Org.ORG_ID = OrgUkprn.ORG_ID
							LEFT OUTER JOIN [VORG_ADDRESSES] AS OrgAddress on Org.ORG_ID = OrgAddress.ORG_ID  AND OrgAddress.ORG_ADDRESS_TYPE_ID = {MAIN_CORRESPONDENCE_ADDRESSS}
							LEFT OUTER JOIN [VADDRESSES] AS Addresses on OrgAddress.ADDRESS_ID  = Addresses.ADDRESS_ID
                        WHERE
	                         OrgUkprn.UKPRN = @ukPrn";

            try
            {
                using (var pimsContext = _pimsContextFactory())
                {
                    var result = await pimsContext.FromSqlAsync<ProviderAddress>(CommandType.Text, sql, new { ukprn });
                    providerAddress = result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Retrieving Provider Address", ex);
            }

            return providerAddress;
        }

        private async Task<Func<IJobQueueDataContext, Task<ProviderManipulationDTO>>> BuildProviderFunc(Organisation organisation)
        {
            return async c => await c.Organisation
                   .Select(s => new ProviderManipulationDTO()
                   {
                       Name = organisation.Name,
                       UKPRN = s.Ukprn,
                       IsMCA = s.IsMca
                   })
                   .SingleOrDefaultAsync(s => s.UKPRN == organisation.Ukprn);
        }

        private async Task<Func<IJobQueueDataContext, Task<EditProviderAssignmentsDTO>>> BuildAssignmentsFunc(OrganisationCollection organisationCollection, long ukprn)
        {
            return async c => await c.OrganisationCollection
                    .Select(s => new EditProviderAssignmentsDTO()
                    {
                        Ukprn = s.Ukprn,
                        CollectionId = s.CollectionId,
                        StartDateUTC = s.StartDateTimeUtc,
                        EndDateUTC = s.EndDateTimeUtc
                    })
                    .SingleOrDefaultAsync(s => s.Ukprn == ukprn && s.CollectionId == organisationCollection.CollectionId);
        }

        private List<Data.Entities.OrganisationCollection> SetupIds(List<OrganisationCollection> organisationCollections, Dictionary<long, int> organisationUkprnDictionary, Dictionary<string, int> collectionNameDictionary)
        {
            var result = new List<Data.Entities.OrganisationCollection>();

            foreach (var organisationCollection in organisationCollections)
            {
                result.Add(new Data.Entities.OrganisationCollection
                {
                    OrganisationId = organisationUkprnDictionary[organisationCollection.Ukprn],
                    CollectionId = collectionNameDictionary[organisationCollection.CollectionName],
                    Ukprn = organisationCollection.Ukprn,
                    StartDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(organisationCollection.StartDate),
                    EndDateTimeUtc = organisationCollection.EndDate.HasValue ? (DateTime?)_dateTimeProvider.ConvertUkToUtc(organisationCollection.EndDate.Value) : null,
                });
            }

            return result;
        }
    }
}
