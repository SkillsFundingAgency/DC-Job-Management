using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.Models;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.CovidRelief.Services.Interfaces
{
    public interface ICovidReliefSubmissionService
    {
        Task SubmitAsync(Submission submission);

        Task<bool> HasExistingSubmissionAsync(long ukprn, int collectionId, int periodNumber);

        Task<IEnumerable<SubmissionDate>> GetSubmissionsByDate(string collectionType, DateTime dateTime);

        Task<List<Submission>> GetSubmissionsListAsync(DateTime dateTimeFromUtc);

        Task<Submission> GetSubmissionDetailsAsync(int submissionId);

        Task<CovidReliefAEBMonthlyData> GetCovidReliefAEBMonthlyDataAsync(long ukprn);

        Task<CovidReliefNLMonthlyData> GetCovidReliefNLMonthlyDataAsync(long ukprn);
    }
}
