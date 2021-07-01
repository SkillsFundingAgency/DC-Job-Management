using System.Collections.Generic;

namespace ESFA.DC.DashBoard.Models.Job
{
    public class DasPaymentDifferencesModel
    {
        public IEnumerable<long> UKPrns { get; set; }

        public int CollectionYear { get; set; }

        public int PeriodNumber { get; set; }
    }
}
