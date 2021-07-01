using System.Collections.Generic;

namespace ESFA.DC.DashBoard.Models.ServiceBus
{
    public sealed class ServiceBusStatusModel
    {
        public IEnumerable<ServiceBusEntityModel> Queues { get; set; }

        public IEnumerable<ServiceBusEntityModel> Topics { get; set; }

        public IEnumerable<ServiceBusEntityModel> Ilr { get; set; }
    }
}