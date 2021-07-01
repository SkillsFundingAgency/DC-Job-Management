using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Data.Services
{
    public class Ilr1920GetSubmissionDetailsService : IIlrGetSubmissionDetailsService
    {
        private readonly Func<IIlr1920RulebaseContext> _ilr1920RulebaseFactory;
        private readonly ILogger _logger;

        public Ilr1920GetSubmissionDetailsService(Func<IIlr1920RulebaseContext> ilr1920RuleBaseFactory)
        {
            _ilr1920RulebaseFactory = ilr1920RuleBaseFactory;
        }

        public int AcademicYear => 1920;

        public async Task<IlrFileDetails> GetIlrSubmissionDetails(long ukprn)
        {
            using (var context = _ilr1920RulebaseFactory())
            {
                var data = await context.FileDetails.Where(x => x.UKPRN == ukprn).OrderByDescending(x => x.SubmittedTime)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    return null;
                }

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
