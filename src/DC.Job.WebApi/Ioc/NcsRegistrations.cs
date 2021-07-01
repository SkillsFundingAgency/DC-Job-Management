using Autofac;
using ESFA.DC.JobQueueManager;
using ESFA.DC.JobQueueManager.Interfaces;

namespace ESFA.DC.Job.WebApi.Ioc
{
    public class NcsRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NcsJobManager>().As<INcsJobManager>();
        }
    }
}
