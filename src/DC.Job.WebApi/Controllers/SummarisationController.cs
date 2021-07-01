using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/summarisation")]
    public class SummarisationController : Controller
    {
        private readonly ISummarisationDataService _summarisationDataService;

        public SummarisationController(
            ISummarisationDataService summarisationDataService)
        {
            _summarisationDataService = summarisationDataService;
        }

        [HttpGet("return-codes/{collectionType}/{maxCollectionsCodesCount?}/{dateTimeUntil?}")]
        public async Task<List<SummarisationCollectionReturnCode>> GetAsync(string collectionType, CancellationToken cancellationToken, int? maxCollectionsCodesCount = null, DateTime? dateTimeUntil = null)
        {
            return await _summarisationDataService.GetLatestSummarisationCollectionCodesAsync(collectionType, cancellationToken, maxCollectionsCodesCount, dateTimeUntil);
        }

        [HttpGet("return-codes-for-period/{collectionType}/{year}/{period}/{pathItemId}")]
        public async Task<List<SummarisationCollectionReturnCode>> GetReturnCodeForPeriodAsync(string collectionType, int year, int period, int pathItemId, CancellationToken cancellationToken)
        {
            return await _summarisationDataService.GetReturnCodeForPeriodAsync(collectionType, year, period, pathItemId, cancellationToken);
        }

        [HttpGet("return-totals")]
        public async Task<List<SummarisationTotal>> GetTotalsAsync([FromQuery] List<int> collectionReturnIds, CancellationToken cancellationToken)
        {
            return await _summarisationDataService.GetSummarisationTotalsAsync(collectionReturnIds, cancellationToken);
        }
    }
}