using System.IO;
using System.Linq;
using ESFA.DC.JobQueueManager.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.AcceptanceTests
{
    public class TopicsTasksDataTests
    {
        private readonly ITestOutputHelper output;

        public TopicsTasksDataTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            return config;
        }

        [Fact]
        public void TestJobTopicTasksData_UniqueTopicOrder()
        {
            using (var context = new JobQueueDataContext(GetContextOptions()))
            {
                var data = context.JobTopicSubscription.Where(x => x.Enabled.GetValueOrDefault())
                    .GroupBy(x => new { x.CollectionId, x.TopicOrder, x.IsFirstStage }).Select(g => g.Count());

                data.Any(x => x > 1).Should().BeFalse();
            }
        }

        [Fact]
        public void TestJobTopicTasksData_UniqueSubscriptionName()
        {
            using (var context = new JobQueueDataContext(GetContextOptions()))
            {
                var data = context.JobTopicSubscription.Where(x => x.Enabled.GetValueOrDefault())
                    .GroupBy(x => new { x.CollectionId, x.SubscriptionName, x.IsFirstStage, x.TopicOrder }).Select(g => new { g.Key.CollectionId, g.Key.SubscriptionName, Kount = g.Count() });

                output.WriteLine(string.Join(",", data.Where(x => x.Kount > 1).Select(x => $"{x.CollectionId} {x.SubscriptionName}").ToArray()));
                data.Any(x => x.Kount > 1).Should().BeFalse();
            }
        }

        [Fact]
        public void TestJobTopicTasksData_AtLeastOneEnabledTopic()
        {
            using (var context = new JobQueueDataContext(GetContextOptions()))
            {
                var data = context.JobTopicSubscription.Where(x => x.Enabled.GetValueOrDefault())
                    .GroupBy(x => new { x.CollectionId, x.IsFirstStage }).Select(g => g.Count());

                data.Any(x => x > 0).Should().BeTrue();
            }
        }

        private DbContextOptions<JobQueueDataContext> GetContextOptions()
        {
            var connectionString = GetConfiguration().GetConnectionString("JobQueueManager");
            DbContextOptionsBuilder<JobQueueDataContext> optionsBuilder =
                new DbContextOptionsBuilder<JobQueueDataContext>()
                    .UseSqlServer(connectionString);

            return optionsBuilder.Options;
        }
    }
}
