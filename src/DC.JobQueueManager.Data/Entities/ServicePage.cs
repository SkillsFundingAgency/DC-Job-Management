using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ServicePage
    {
        public ServicePage()
        {
            ServicePageMessage = new HashSet<ServicePageMessage>();
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ControllerName { get; set; }

        public virtual ICollection<ServicePageMessage> ServicePageMessage { get; set; }
    }
}
