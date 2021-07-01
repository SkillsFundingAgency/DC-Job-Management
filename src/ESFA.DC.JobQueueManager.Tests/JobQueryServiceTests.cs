using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Job = ESFA.DC.Jobs.Model.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class JobQueryServiceTests : AbstractBaseJobQueueManagerTests
    {
        public async Task Get_UnsubmittedFiles()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);

                    context.Collection.Add(new Collection()
                    {
                        CollectionId = 2,
                        CollectionTypeId = 1,
                        Name = CollectionConstants.ILR1920,
                        MultiStageProcessing = true,
                        CollectionYear = 1920,
                    });
                    await context.SaveChangesAsync();
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    CollectionName = CollectionConstants.ILR1819,
                    PeriodNumber = 11,
                    Ukprn = 100,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    CollectionName = CollectionConstants.ILR1819,
                    PeriodNumber = 10,
                    Ukprn = 100,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 3,
                    CollectionName = CollectionConstants.ILR2021,
                    PeriodNumber = 1,
                    Ukprn = 100,
                });

                var jobQueryService = scope.Resolve<IJobQueryService>();
                var result = await jobQueryService.GetUnsubmittedIlrJobsAsync(100, CancellationToken.None);

                result.Should().NotBeNull();
                result.Count().Should().Be(2);
                result.SingleOrDefault(x => x.CollectionName == CollectionConstants.ILR1819 && x.PeriodNumber == 11).Should().NotBeNull();
                result.SingleOrDefault(x => x.CollectionName == CollectionConstants.ILR1920 && x.PeriodNumber == 1).Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(JobStatusType.Processing, true)]
        [InlineData(JobStatusType.Ready, true)]
        [InlineData(JobStatusType.MovedForProcessing, true)]
        [InlineData(JobStatusType.Failed, false)]
        [InlineData(JobStatusType.FailedRetry, false)]
        [InlineData(JobStatusType.Waiting, false)]
        [InlineData(JobStatusType.Completed, false)]
        public async Task IsAnyJobInProgress_Test(JobStatusType jobStatus, bool expectedResult)
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, "REF-ULN");

                    var job = new Job
                    {
                        JobId = 1,
                        CollectionName = "REF-ULN",
                        Status = jobStatus,
                        CreatedBy = "test",
                    };

                    var manager = scope.Resolve<IJobManager>();
                    var queryService = scope.Resolve<IJobQueryService>();

                    await manager.AddJob(job);

                    var result = await queryService.IsAnyJobInProgressAsync(1, null, CancellationToken.None);
                    result.Should().Be(expectedResult);
                }
            }
        }

        [Fact]
        public async Task GetCollectionTypesWithSubmissionsForProviderAsync_Test()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    await context.Database.EnsureCreatedAsync();

                    var collectionTypeIlr = new CollectionType
                    {
                        CollectionTypeId = 1,
                        Description = "submission",
                        Type = "ILR"
                    };
                    var collectionTypeFunded = new CollectionType
                    {
                        CollectionTypeId = 2,
                        Description = "funded aims",
                        Type = "MCA-GLA-FA"
                    };

                    context.CollectionType.Add(collectionTypeIlr);
                    context.CollectionType.Add(collectionTypeFunded);

                    var collection1 = new Collection
                    {
                        CollectionType = collectionTypeIlr,
                        Name = CollectionConstants.ILR1920,
                        CollectionYear = 1920,
                        CollectionId = 1
                    };

                    var collection2 = new Collection
                    {
                        CollectionType = collectionTypeFunded,
                        Name = "MCA-GLA-FundedAims",
                        CollectionYear = 2021,
                        CollectionId = 2
                    };

                    context.Collection.Add(collection1);
                    context.Collection.Add(collection2);

                    context.Job.Add(new Data.Entities.Job()
                    {
                        Collection = collection2,
                        Ukprn = 10000,
                        JobId = 1,
                    });

                    context.Job.Add(new Data.Entities.Job()
                    {
                        Collection = collection2,
                        Ukprn = 10000,
                        JobId = 2,
                    });

                    await context.SaveChangesAsync();
                }

                var returnCalendarService = scope.Resolve<IJobQueryService>();
                var data = await returnCalendarService.GetCollectionTypesWithSubmissionsForProviderAsync(10000, CancellationToken.None);
                data.Should().NotBeNull();

                data.Contains("ILR").Should().BeFalse();
                data.Contains("MCA-GLA-FA").Should().BeTrue();
            }
        }
    }
}
