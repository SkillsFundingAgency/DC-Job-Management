using System;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndDateTimeService
    {
        TimeSpan CalculateRuntimeSimple(DateTime? startDate, DateTime? endDate);

        Task<PeriodEndWeekendModel> CalculateRuntimeBusiness(DateTime? startDate, DateTime? endDate);
    }
}
