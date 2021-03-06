using Autofac;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobScheduler.Console.Ioc;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ESFA.DC.JobScheduler.Console.Tests
{
    public class IocTests
    {
        [Fact]
        public void Test_AutofacConnfiguration()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ServiceRegistrations>();

            using (var container = containerBuilder.Build())
            {
                container.IsRegistered(typeof(IMessagingService)).Should().BeTrue();
                container.IsRegistered(typeof(IJobManager)).Should().BeTrue();
                container.IsRegistered(typeof(IFileUploadJobManager)).Should().BeTrue();
                container.IsRegistered(typeof(INcsJobManager)).Should().BeTrue();
                container.IsRegistered(typeof(IJobQueueHandler)).Should().BeTrue();
                container.IsRegistered(typeof(IJobSchedulerStatusManager)).Should().BeTrue();
                // container.IsRegistered(typeof(ITopicPublishService<JobContextDto>)).Should().BeTrue();
                container.IsRegistered(typeof(IJsonSerializationService)).Should().BeTrue();
                container.IsRegistered(typeof(IAuditor)).Should().BeTrue();
                container.IsRegistered(typeof(DbContextOptions<JobQueueDataContext>)).Should().BeTrue();
                container.IsRegistered(typeof(ILogger)).Should().BeTrue();
            }
        }
    }
}
