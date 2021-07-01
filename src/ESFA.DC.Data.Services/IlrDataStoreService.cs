using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.Data.Services
{
    public class IlrDataStoreService : IIlrDataStoreService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IIlrGetSubmissionDetailsService> _ilrSubmissionDetailsServices;

        public IlrDataStoreService(ILogger logger, IEnumerable<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsServices)
        {
            _logger = logger;
            _ilrSubmissionDetailsServices = ilrSubmissionDetailsServices;
        }

        public async Task<IlrFileDetails> GetLatestIlrSubmissionDetails(int academicYear, long ukprn)
        {
            try
            {
                var ilrSubmissionDetailsService = _ilrSubmissionDetailsServices.Single(s => s.AcademicYear == academicYear);
                return await ilrSubmissionDetailsService.GetIlrSubmissionDetails(ukprn);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured trying to get file details for ukprn : {ukprn}", e);
                throw;
            }
        }
    }
}
