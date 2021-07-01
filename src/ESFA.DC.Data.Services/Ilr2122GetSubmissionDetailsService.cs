using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.ILR2122.DataStore.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Data.Services
{
    public class Ilr2122GetSubmissionDetailsService : IIlrGetSubmissionDetailsService
    {
        private readonly Func<IIlr2122Context> _ilr2122Factory;
        private readonly ILogger _logger;

        public Ilr2122GetSubmissionDetailsService(Func<IIlr2122Context> ilr2122Factory, ILogger logger)
        {
            _ilr2122Factory = ilr2122Factory;
            _logger = logger;
        }

        public int AcademicYear => 2122;

        public async Task<IlrFileDetails> GetIlrSubmissionDetails(long ukprn)
        {
            using (var context = _ilr2122Factory())
            {
                _logger.LogInfo($"Finding latest ILR{AcademicYear} file details for UKPRN {ukprn}.");
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
