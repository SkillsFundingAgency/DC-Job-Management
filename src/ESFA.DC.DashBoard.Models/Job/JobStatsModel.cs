using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.DashBoard.Models.Job
{
    public sealed class JobStatsModel
    {
        public IEnumerable<TodayStatsForYearModel> TodayStatsForYearModel { get; set; }

        public SlowFilesComparedToThreePreviousModel SlowFilesComparedToThreePreviousModel { get; set; }

        public IEnumerable<JobsCurrentPeriodModel> JobsCurrentPeriodModels { get; set; }

        public IEnumerable<CurrentPeriodIlrModel> CurrentPeriodIlrModels { get; set; }

        public IEnumerable<CurrentPeriodSuccessfulIlrModel> CurrentPeriodSuccessfulUkprnsModels { get; set; }

        public IEnumerable<DasPaymentDifferencesModel> DasPaymentDifferencesModels { get; set; }

        public ConcernsModel ConcernsModel { get; set; }

        public TodayProcessingTimeModel TodayProcessingTimeModel { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }
    }
}