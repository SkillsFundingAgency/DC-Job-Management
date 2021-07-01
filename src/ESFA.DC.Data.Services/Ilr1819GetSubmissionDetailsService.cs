using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Data.Services
{
    public class Ilr1819GetSubmissionDetailsService : IIlrGetSubmissionDetailsService
    {
        private readonly Func<IIlr1819RulebaseContext> _ilr1819RulebaseFactory;
        private readonly ILogger _logger;

        public Ilr1819GetSubmissionDetailsService(Func<IIlr1819RulebaseContext> ilr1819RuleBaseFactory)
        {
            _ilr1819RulebaseFactory = ilr1819RuleBaseFactory;
        }

        public int AcademicYear => 1819;

        public async Task<IlrFileDetails> GetIlrSubmissionDetails(long ukprn)
        {
            using (var context = _ilr1819RulebaseFactory())
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
