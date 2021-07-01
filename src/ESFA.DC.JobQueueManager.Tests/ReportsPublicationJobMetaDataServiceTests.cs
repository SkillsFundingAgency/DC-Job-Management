using System.Linq;
using System.Threading;
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
    public class ReportsPublicationJobMetaDataServiceTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task PublishReportsTest()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsPublication(context, "dea1920", collectionYear: 1920);

                    var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                    var job = await manager.AddJob(new ReportsPublicationJob()
                    {
                        JobId = 1,
                        CollectionName = "dea1920",
                        SourceContainerName = "test1",
                        SourceFolderKey = "test2",
                        StorageReference = "StorageTest",
                        PeriodNumber = 2,
                    });

                    var service = scope.Resolve<IReportsPublicationJobMetaDataService>();
                    await service.MarkAsPublishedAsync(1, CancellationToken.None);

                    // Assert
                    var data = await service.GetReportsPublicationDataAsync(CancellationToken.None, "dea1920");
                    data.Should().NotBeNullOrEmpty();

                    var dataItem = data.First();
                    dataItem.CollectionName.Should().Be("dea1920");
                    dataItem.PeriodNumber.Should().Be(2);
                    dataItem.CollectionYear.Should().Be(1920);
                }
            }
        }

        [Fact]
        public async Task UnpublishFrmReportsTest()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsPublication(context, "dea1920", collectionYear: 1920);

                    var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                    var job = await manager.AddJob(new ReportsPublicationJob()
                    {
                        JobId = 1,
                        CollectionName = "dea1920",
                        SourceContainerName = "test1",
                        SourceFolderKey = "test2",
                        StorageReference = "StorageTest",
                        PeriodNumber = 2,
                    });

                    var service = scope.Resolve<IReportsPublicationJobMetaDataService>();
                    await service.MarkAsPublishedAsync(1, CancellationToken.None);

                    await service.MarkAsUnPublishedAsync("dea1920", 2, CancellationToken.None);

                    // Assert
                    var data = context.ReportsPublicationJobMetaData.Where(x =>
                        x.Job.Collection.Name == "dea1920" && x.PeriodNumber == 2 && x.ReportsPublished == false).ToList();
                    data.Should().NotBeNullOrEmpty();

                    data.Count.Should().Be(1);
                }
            }
        }
    }
}
