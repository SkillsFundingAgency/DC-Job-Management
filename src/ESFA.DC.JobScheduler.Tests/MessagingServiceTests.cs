using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobScheduler.Interfaces.Models;
using ESFA.DC.Queueing.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.JobScheduler.Tests
{
    public class MessagingServiceTests
    {
        [Fact(Skip = "Need to see if using topics like this is the correct way")]
        public void SendMessagesAsync_Test()
        {
            var topicPublishMock = new Mock<ITopicPublishService<JobContextDto>>();
            topicPublishMock.Setup(x => x.PublishAsync(new JobContextDto(), new Dictionary<string, object>(), "Test")).Returns(Task.CompletedTask);

            var message = new MessageParameters("ILR1819")
            {
                JobContextMessage = new JobContextMessage()
                {
                    KeyValuePairs = new Dictionary<string, object>(),
                    Topics = new List<ITopicItem>()
                },
                SubscriptionLabel = "Test",
                TopicParameters = new Dictionary<string, object>(),
            };

            var indexedTopicsMock = new Mock<IIndex<EnumJobType, ITopicPublishService<JobContextDto>>>();
            indexedTopicsMock.SetupGet(x => x[EnumJobType.IlrSubmission]).Returns(topicPublishMock.Object);

            var messagingService = new MessagingService(new JobContextMapper(), null, null, null, null);
            messagingService.SendMessageAsync(message).ConfigureAwait(true);

            topicPublishMock.Verify(x => x.PublishAsync(It.IsAny<JobContextDto>(), It.IsAny<Dictionary<string, object>>(), "Test"), Times.Once);
        }
    }
}
