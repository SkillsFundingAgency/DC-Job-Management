using System;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.Models;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/covid-relief")]
    public class CovidReliefController : Controller
    {
        private readonly ICovidReliefSubmissionService _covidReliefSubmissionService;
        private readonly ILogger _logger;

        public CovidReliefController(ICovidReliefSubmissionService covidReliefSubmissionService, ILogger logger)
        {
            _covidReliefSubmissionService = covidReliefSubmissionService;
            _logger = logger;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitData([FromBody] Submission submission)
        {
            try
            {
                if (submission == null || submission.Ukprn <= 0)
                {
                    _logger.LogError($"Post call received with bad data");
                    return BadRequest();
                }

                await _covidReliefSubmissionService.SubmitAsync(submission);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for posting covid relief submission data for ukprn : {submission.Ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet("has-any-submission/{ukprn}/{collectionId}/{periodNumber}")]
        public async Task<IActionResult> HasAnySubmission(long ukprn, int collectionId, int periodNumber)
        {
            try
            {
                if (ukprn < 0 || periodNumber <= 0 || collectionId <= 0)
                {
                    _logger.LogError($"Get call received with bad data");
                    return BadRequest();
                }

                var result = await _covidReliefSubmissionService.HasExistingSubmissionAsync(ukprn, collectionId, periodNumber);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for checking if covid relief submission exists for ukprn : {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet("submissions-list/{dateTimeFromUtc}")]
        public async Task<IActionResult> GetSubmissions(DateTime dateTimeFromUtc)
        {
            try
            {
                var result = await _covidReliefSubmissionService.GetSubmissionsListAsync(dateTimeFromUtc);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for getting submissions list for covid relief submission", e);
                return BadRequest();
            }
        }

        [HttpGet("{submissionId}")]
        public async Task<IActionResult> GetSubmission(int submissionId)
        {
            try
            {
                if (submissionId <= 0)
                {
                    return BadRequest("submission id supplied is invalid");
                }

                var result = await _covidReliefSubmissionService.GetSubmissionDetailsAsync(submissionId);

                if (result != null)
                {
                    return Ok(result);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for getting submission details  for covid relief submission", e);
                return BadRequest();
            }
        }

        [HttpGet("caps/aeb/{ukprn}")]
        public async Task<IActionResult> GetAebMonthlyCaps(long ukprn)
        {
            try
            {
                var result = await _covidReliefSubmissionService.GetCovidReliefAEBMonthlyDataAsync(ukprn);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for getting GetAebMonthlyCaps : ukprn {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet("caps/nl/{ukprn}")]
        public async Task<IActionResult> GetNLMonthlyCaps(long ukprn)
        {
            try
            {
                var result = await _covidReliefSubmissionService.GetCovidReliefNLMonthlyDataAsync(ukprn);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured for getting GetNLMonthlyCaps : ukprn {ukprn}", e);
                return BadRequest();
            }
        }
    }
}