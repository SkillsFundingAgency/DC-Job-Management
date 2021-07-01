using ESFA.DC.JobStatus.Interfaces;

namespace ESFA.DC.JobStatus
{
    public sealed class JobStatusWebServiceCallServiceConfig : IJobStatusWebServiceCallServiceConfig
    {
        public JobStatusWebServiceCallServiceConfig(string endPointUrl)
        {
            EndPointUrl = endPointUrl;
        }

        public string EndPointUrl { get; }
    }
}
