using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/returns-calendar")]
    public class ReturnsCalendarController : Controller
    {
        private readonly IReturnCalendarService _returnCalendarService;

        public ReturnsCalendarController(
            IReturnCalendarService returnCalendarService)
        {
            _returnCalendarService = returnCalendarService;
        }

        // GET api/values/5
        [HttpGet("{collectionName}/current")]
        public async Task<ReturnPeriod> GetCurrent(string collectionName)
        {
            return await _returnCalendarService.GetCurrentPeriodAsync(collectionName);
        }

        // GET api/values/5
        [HttpGet("{collectionName}/next")]
        public async Task<ReturnPeriod> GetNext(string collectionName)
        {
            return await _returnCalendarService.GetNextPeriodAsync(collectionName);
        }

        // GET api/values/5
        [HttpGet("{collectionName}/{dateTimeUtc}")]
        public async Task<ReturnPeriod> GetPeriod(string collectionName, DateTime dateTimeUtc)
        {
            return await _returnCalendarService.GetPeriodAsync(collectionName, dateTimeUtc);
        }

        // GET api/values/5
        [HttpGet("{collectionName}/previous/{dateTimeUtc}")]
        public async Task<ReturnPeriod> GetPreviousPeriod(string collectionName, DateTime dateTimeUtc)
        {
            return await _returnCalendarService.GetPreviousPeriodAsync(collectionName, dateTimeUtc);
        }

        // GET api/values/5
        [HttpGet("closed/{collectionType?}/{dateTimeUtc?}")]
        public async Task<ReturnPeriod> GetRecentlyClosedPeriod(string collectionType = CollectionTypeConstants.Ilr, DateTime? dateTimeUtc = null)
        {
            return await _returnCalendarService.GetRecentlyClosedPeriodAsync(dateTimeUtc, collectionType);
        }

        // GET api/values/5
        [HttpGet("open/{collectionType?}/{dateTimeUtc?}")]
        public async Task<List<ReturnPeriod>> GetOpenPeriods(string collectionType = CollectionTypeConstants.Ilr, DateTime? dateTimeUtc = null)
        {
            return await _returnCalendarService.GetOpenPeriodsAsync(dateTimeUtc, collectionType);
        }

        // GET api/values/5
        [HttpGet("all/{collectionType}")]
        public async Task<List<ReturnPeriod>> GetAllPeriods(string collectionType)
        {
            return (await _returnCalendarService.GetAllPeriodsAsync(null, collectionType)).ToList();
        }

        // GET api/values/5
        [HttpGet("{collectionName}")]
        public async Task<List<ReturnPeriod>> GetAllPeriodsAsync(string collectionName)
        {
            return (await _returnCalendarService.GetAllPeriodsAsync(collectionName)).ToList();
        }

        [HttpGet("periodEnd/{collectionType}")]
        public async Task<YearPeriod> GetPeriodEndPeriod(string collectionType)
        {
            return await _returnCalendarService.GetPeriodEndPeriod(collectionType);
        }

        [HttpGet("expired/{collectionName}")]
        public async Task<bool> IsReferenceDataCollectionExpiredAsync(string collectionName, CancellationToken cancellationToken)
        {
            return await _returnCalendarService.IsReferenceDataCollectionExpiredAsync(collectionName, cancellationToken);
        }

        [HttpGet("next-closing/{collectionType}")]
        public async Task<ReturnPeriod> GetNextClosingPeriod(string collectionType, CancellationToken cancellationToken)
        {
            return await _returnCalendarService.GetNextClosingPeriodAsync(collectionType, cancellationToken);
        }
    }
}