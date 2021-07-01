using ESFA.DC.DashBoard.Models.Job;
using ESFA.DC.DashBoard.Models.ServiceBus;

namespace ESFA.DC.DashBoard.Models
{
    public sealed class DashBoardModel
    {
        public ServiceBusStatusModel ServiceBusStats { get; set; }

        public JobStatsModel JobStats { get; set; }
    }
}