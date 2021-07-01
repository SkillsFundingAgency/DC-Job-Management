using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Data.Services
{
    public class Ilr2021GetSubmissionDetailsService : IIlrGetSubmissionDetailsService
    {
        private readonly Func<IIlr2021RulebaseContext> _ilr2021RuleBaseFactory;
        private readonly ILogger _logger;

        public Ilr2021GetSubmissionDetailsService(Func<IIlr2021RulebaseContext> ilr2021RuleBaseFactory, ILogger logger)
        {
            _ilr2021RuleBaseFactory = ilr2021RuleBaseFactory;
            _logger = logger;
        }

        public int AcademicYear => 2021;

        public async Task<IlrFileDetails> GetIlrSubmissionDetails(long ukprn)
        {
            _logger.LogInfo($"Finding latest ILR{AcademicYear} file details for UKPRN {ukprn}.");
            using (var context = _ilr2021RuleBaseFactory())
            {
                var data = await context.FileDetails.Where(x => x.UKPRN == ukprn).OrderByDescending(x => x.SubmittedTime)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    _logger.LogInfo($"No previous ILR{AcademicYear} file details for UKPRN {ukprn}.");
                    return null;
                }

                _logger.LogInfo($"Found latest ILR{AcademicYear} file details for UKPRN {ukprn}. File: {data.Filename}.");
                return new IlrFileDetails()
                {
                    SubmittedTime = data.SubmittedTime.GetValueOrDefault(),
                    FileSizeKb = data.FileSizeKb.GetValueOrDefault(),
                    Filename = data.Filename,
                    TotalErrorCount = data.TotalErrorCount.GetValueOrDefault(),
                    TotalInvalidLearnersSubmitted = data.TotalInvalidLearnersSubmitted.GetValueOrDefault(),
                    TotalLearnersSubmitted = data.TotalLearnersSubmitted.GetValueOrDefault(),
                    TotalValidLearnersSubmitted = data.TotalValidLearnersSubmitted.GetValueOrDefault(),
                    TotalWarningCount = data.TotalWarningCount.GetValueOrDefault(),
                    Ukprn = data.UKPRN
                };
            }
        }
    }
}
