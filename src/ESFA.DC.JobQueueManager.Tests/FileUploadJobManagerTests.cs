using System;
using System.Collections.Generic;
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
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class FileUploadJobManagerTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task AddJob_Null()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
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
                    SetupCollections(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                var result = await manager.AddJob(new FileUploadJob
                {
                    JobId = 1,
                    CollectionName = CollectionConstants.ILR2021,
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
                    SetupCollections(context);

                    FileUploadJob job = new FileUploadJob
                    {
                        JobId = 1,
                        CollectionName = CollectionConstants.ILR2021,
                        Ukprn = 1000,
                        FileName = "test.xml",
                        StorageReference = "test-ref",
                        FileSize = 10.5m,
                        IsFirstStage = true,
                        PeriodNumber = 10,
                        Status = JobStatusType.Ready,
                        CreatedBy = "test",
                        CrossLoadingStatus = 0,
                        NotifyEmail = "test@test.com",
                        Priority = 2,
                    };

                    IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                    var queryService = scope.Resolve<IJobQueryService>();

                    await manager.AddJob(job);

                    savedJob = await queryService.GetJobById(1);
                }
            }

            savedJob.Should().NotBeNull();
            savedJob.JobId.Should().Be(1);
            savedJob.Ukprn.Should().Be(1000);
            savedJob.FileName.Should().Be("test.xml");
            savedJob.FileSize.Should().Be(10.5m);
            savedJob.StorageReference.Should().Be("test-ref");
            savedJob.IsSubmitted.Should().Be(false);
            savedJob.CollectionName.Should().Be(CollectionConstants.ILR2021);
            savedJob.PeriodNumber.Should().Be(10);
            savedJob.CollectionYear.Should().Be(1819);
            savedJob.NotifyEmail.Should().Be("test@test.com");
            savedJob.CollectionType.Should().Be("ILR");
            savedJob.CreatedBy.Should().Be("test");
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
                    SetupCollections(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                var queryService = scope.Resolve<IJobQueryService>();

                await manager.AddJob(new FileUploadJob
                {
                    JobId = 1,
                    CollectionName = CollectionConstants.ILR2021,
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

                var queryService = scope.Resolve<IJobQueryService>();

                (await queryService.GetJobById(100)).Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateJobStage_Fail_Zero()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                }

                var manager = scope.Resolve<IFileUploadJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.SubmitIlrJob(0));
            }
        }

        [Fact]
        public async Task UpdateJob_Fail_InvalidJobId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.SubmitIlrJob(100));
            }
        }

        [Fact]
        public async Task GetJobsByUkprn_Success()
        {
            IContainer container = GetRegistrations();
            List<SubmittedJob> result;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();

                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    Ukprn = 100,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 100,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 999900,
                    CollectionName = CollectionConstants.ILR2021,
                });

                var queryService = scope.Resolve<IJobQueryService>();

                result = (await queryService.GetJobsByUkprnAsync(100, CancellationToken.None)).ToList();
            }

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetJobsByUkprnForDateRange_Success()
        {
            IContainer container = GetRegistrations();
            List<SubmittedJob> result;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollectionsNotMulti(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();

                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    Ukprn = 100,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 100,
                    CollectionName = CollectionConstants.ILR2021,
                });

                var queryService = scope.Resolve<IJobQueryService>();

                result = (await queryService.GetJobsByUkprnForDateRangeAsync(100, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow, CancellationToken.None)).ToList();
            }

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetJobsByUkprnForPeriod_Success()
        {
            IContainer container = GetRegistrations();
            List<SubmittedJob> result;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();

                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    Ukprn = 100,
                    PeriodNumber = 1,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 999900,
                    PeriodNumber = 2,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 999900,
                    PeriodNumber = 2,
                    CollectionName = CollectionConstants.ILR2021,
                });

                var queryService = scope.Resolve<IJobQueryService>();
                result = (await queryService.GetJobsByUkprnForPeriodAsync(999900, 2, CancellationToken.None)).ToList();
            }

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetLatestJobByUkprnAndContractReference_Success()
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
                    SetupCollections(context, "ESFR1");
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();

                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    Ukprn = 10000116,
                    PeriodNumber = 1,
                    FileName = "10000116/SUPPDATA-10000116-ESF-2270-20181109-090919.csv",
                    CollectionName = "ESFR1",
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 10000116,
                    PeriodNumber = 2,
                    FileName = "10000116/SUPPDATA-10000116-ESF-99999-20181109-090919.csv",
                    CollectionName = "ESFR1",
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 3,
                    Ukprn = 10000119,
                    PeriodNumber = 2,
                    FileName = "10000119/SUPPDATA-10000119-ESF-2270-20181109-090919.csv",
                    CollectionName = "ESFR1",
                });

                var queryService = scope.Resolve<IJobQueryService>();

                result = await queryService.GetLatestJobByUkprnAndContractReferenceAsync(10000116, "ESF-2270", "ESFR1", CancellationToken.None);
            }

            result.Should().NotBeNull();
            result.FileName.Should().Be("10000116/SUPPDATA-10000116-ESF-2270-20181109-090919.csv");
            result.Ukprn.Should().Be(10000116);
            result.JobId.Should().Be(1);
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
                    SetupCollections(context);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    CollectionName = CollectionConstants.ILR2021,
                    Ukprn = 100,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    CollectionName = CollectionConstants.ILR2021,
                    Ukprn = 100,
                });
                await manager.AddJob(new FileUploadJob()
                {
                    JobId = 3,
                    CollectionName = CollectionConstants.ILR2021,
                    Ukprn = 999900,
                });

                var result = (await manager.GetAllJobs()).ToList();
                result.Should().NotBeNull();
                result.Count.Should().Be(3);
            }
        }

        [Fact]
        public async Task GetLatestJobsPerPeriodByUkprn_Success()
        {
            IContainer container = GetRegistrations();
            List<SubmittedJob> result;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                    SetupCollections(context, "EAS2021", 2);
                    SetupCollections(context, "ESFR1", 3);
                    SetupCollections(context, "ILR1718", 4, true, 1718);
                }

                IFileUploadJobManager manager = scope.Resolve<IFileUploadJobManager>();

                await manager.AddJob(new FileUploadJob
                {
                    JobId = 1,
                    Ukprn = 10000116,
                    FileName = "esf.csv",
                    CollectionName = "ESFR1",
                    Status = JobStatusType.Completed,
                    PeriodNumber = 1,
                    DateTimeSubmittedUtc = DateTime.Now.AddMinutes(-10),
                });
                await manager.AddJob(new FileUploadJob
                {
                    JobId = 2,
                    Ukprn = 10000116,
                    FileName = "eas.csv",
                    CollectionName = "EAS2021",
                    PeriodNumber = 1,
                    Status = JobStatusType.Completed,
                    DateTimeSubmittedUtc = DateTime.Now.AddMinutes(-10),
                });
                await manager.AddJob(new FileUploadJob
                {
                    JobId = 3,
                    Ukprn = 10000116,
                    FileName = "ILR2021.xml",
                    CollectionName = CollectionConstants.ILR2021,
                    PeriodNumber = 1,
                    Status = JobStatusType.Completed,
                });
                await manager.AddJob(new FileUploadJob
                {
                    JobId = 4,
                    Ukprn = 10000116,
                    FileName = "ilr1718.xml",
                    CollectionName = "ILR1718",
                    PeriodNumber = 1,
                    Status = JobStatusType.Completed,
                });
                await manager.AddJob(new FileUploadJob
                {
                    JobId = 5,
                    Ukprn = 10000116,
                    FileName = "ilr_latest_not_completed.xml",
                    CollectionName = CollectionConstants.ILR2021,
                    PeriodNumber = 1,
                    Status = JobStatusType.Failed,
                    DateTimeSubmittedUtc = DateTime.Now.AddMinutes(-50),
                });

                var queryService = scope.Resolve<IJobQueryService>();
                result =
                    (await queryService.GetLatestJobsPerPeriodByUkprnAsync(10000116, DateTime.Now.AddDays(-1), DateTime.Now, CancellationToken.None))
                    .ToList();
            }

            result.Should().NotBeNull();
            result.Count.Should().Be(4);
            result.Single(x => x.CollectionName == "ESFR1" && x.JobId == 1 && x.FileName == "esf.csv").Should().NotBeNull();
            result.Single(x => x.CollectionName == "EAS2021" && x.JobId == 2 && x.FileName == "eas.csv").Should().NotBeNull();
            result.Single(x => x.CollectionName == CollectionConstants.ILR2021 && x.JobId == 3 && x.FileName == "ILR2021.xml" && x.CollectionYear == 1819).Should().NotBeNull();
            result.Single(x => x.CollectionName == "ILR1718" && x.JobId == 4 && x.FileName == "ilr1718.xml" && x.CollectionYear == 1718).Should().NotBeNull();
        }
    }
}
