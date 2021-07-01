using System;
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
    public class FrmUploadJobManagerTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task AddJob_Null()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddJob(null));
            }
        }

        [Fact]
        public async Task AddJob_Success()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsPublication(context);
                }

                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                var result = await manager.AddJob(new ReportsPublicationJob()
                {
                    JobId = 1,
                    CollectionName = "frm1920-files",
                    SourceContainerName = "test1",
                    SourceFolderKey = "test2",
                    StorageReference = "StorageTest",
                    PeriodNumber = 2
                });
                result.Should().Be(1);
            }
        }

        [Fact]
        public async Task AddJobMetaData_Success_Values()
        {
            IContainer container = GetRegistrations();
            SubmittedJob savedJob;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, "frm1920-files", 1, true, collectionYear: 1920);

                    ReportsPublicationJob publicationJob = new ReportsPublicationJob()
                    {
                        JobId = 1,
                        CollectionName = "frm1920-files",
                        SourceFolderKey = "test2",
                        StorageReference = "StorageTest",
                        PeriodNumber = 2
                    };

                    var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                    var queryService = scope.Resolve<IJobQueryService>();

                    await manager.AddJob(publicationJob);

                    savedJob = await queryService.GetJobById(1);
                }
            }

            savedJob.Should().NotBeNull();
            savedJob.JobId.Should().Be(1);
            savedJob.SourceContainerName.Should().Be("frm1920");
            savedJob.SourceFolderKey.Should().Be("test2");
        }

        [Fact]
        public async Task GetJobMetaDataById_Success()
        {
            IContainer container = GetRegistrations();
            SubmittedJob result;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsPublication(context);
                }

                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                var queryService = scope.Resolve<IJobQueryService>();

                var jobId = await manager.AddJob(new ReportsPublicationJob()
                {
                    JobId = 1,
                    CollectionName = "frm1920-files",
                    SourceFolderKey = "test1",
                    SourceContainerName = "test2",
                    StorageReference = "StorageTest",
                    PeriodNumber = 2
                });

                result = await queryService.GetJobById(1);
            }

            result.Should().NotBeNull();
            result.JobId.Should().Be(1);
        }

        [Fact]
        public async Task GetJobMetDataById_Fail_zeroId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                var queryService = scope.Resolve<IJobQueryService>();

                await Assert.ThrowsAsync<ArgumentException>(() => queryService.GetJobById(0));
            }
        }

        [Fact]
        public async Task GetJobById_Fail_IdNotFound()
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
                }

                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                var queryService = scope.Resolve<IJobQueryService>();

                (await queryService.GetJobById(100)).Should().BeNull();
            }
        }

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
                    SetupCollectionsPublication(context);
                }

                var manager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();
                await manager.AddJob(new ReportsPublicationJob()
                {
                    JobId = 1,
                    CollectionName = "frm1920-files",
                    Ukprn = 100,
                    SourceContainerName = "test1",
                    SourceFolderKey = "test2",
                    StorageReference = "StorageTest",
                    PeriodNumber = 2
                });
                await manager.AddJob(new ReportsPublicationJob()
                {
                    JobId = 2,
                    CollectionName = "frm1920-files",
                    Ukprn = 100,
                    SourceContainerName = "test1",
                    SourceFolderKey = "test2",
                    StorageReference = "StorageTest",
                    PeriodNumber = 2
                });
                await manager.AddJob(new ReportsPublicationJob()
                {
                    JobId = 2,
                    CollectionName = "frm1920-files",
                    SourceContainerName = "test1",
                    SourceFolderKey = "test2",
                    Ukprn = 999900,
                    StorageReference = "StorageTest",
                    PeriodNumber = 2
                });

                var result = (await manager.GetAllJobs()).ToList();

                result.Should().NotBeNull();
                result.Count.Should().Be(3);
            }
        }
    }
}
