using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class ValidationRuleDetailsReportJobManagerTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task GetAllJobs_Success()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsValidationRuleReport(context);
                }

                var manager = scope.Resolve<IUpdateJobManager<ValidationRuleDetailsReportJob>>();
                await manager.AddJob(new ValidationRuleDetailsReportJob()
                {
                    JobId = 1,
                    CollectionName = "OP-Validation-Report",
                    Ukprn = 100,
                    Rule = "Rule1",
                    SelectedCollectionYear = 1920,
                });
                await manager.AddJob(new ValidationRuleDetailsReportJob()
                {
                    JobId = 3,
                    CollectionName = "OP-Validation-Report",
                    Ukprn = 100,
                    Rule = "Rule2",
                    SelectedCollectionYear = 1819,
                });
                await manager.AddJob(new ValidationRuleDetailsReportJob()
                {
                    JobId = 3,
                    CollectionName = "OP-Validation-Report",
                    Ukprn = 100,
                    Rule = "Rule3",
                    SelectedCollectionYear = 1920,
                });

                var result = (await manager.GetAllJobs()).ToList();

                result.Should().NotBeNull();
                result.Count.Should().Be(3);
                result[0].Rule.Should().Be("Rule1");
                result[1].Rule.Should().Be("Rule2");
                result[0].RowVersion.Should().Be(null);
            }
        }
    }
}
