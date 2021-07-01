using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Job = ESFA.DC.Jobs.Model.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class JobManagerTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task AddJob_Null()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                IJobManager manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddJob(null));
            }
        }

        [Theory]
        [InlineData("PE-DAS")]
        [InlineData("EAS2021")]
        [InlineData("REF-ULN")]
        [InlineData("REF-FCS")]
        public async Task AddJob_Success_Values(string collectionName)
        {
            var job = new Job
            {
                DateTimeCreatedUtc = DateTime.UtcNow,
                DateTimeUpdatedUtc = DateTime.UtcNow,
                JobId = 0,
                Priority = 1,
                RowVersion = null,
                Status = JobStatusType.Ready,
                CreatedBy = "test user",
                CollectionName = collectionName,
                NotifyEmail = "test@email.com",
            };

            IContainer container = GetRegistrations();

            Job savedJob;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, collectionName);
                }

                var manager = scope.Resolve<IJobManager>();

                await manager.AddJob(job);

                var result = await manager.GetAllJobs();

                savedJob = result.SingleOrDefault();
            }

            savedJob.Should().NotBeNull();

            savedJob.JobId.Should().Be(1);
            savedJob.DateTimeCreatedUtc.Should().BeOnOrBefore(DateTime.UtcNow);
            savedJob.DateTimeUpdatedUtc.Should().BeNull();
            savedJob.CollectionName.Should().Be(collectionName);
            savedJob.Priority.Should().Be(1);
            savedJob.CreatedBy.Should().Be("test user");
            savedJob.Status.Should().Be(JobStatusType.Ready);
            savedJob.NotifyEmail.Should().Be("test@email.com");
            savedJob.CrossLoadingStatus.Should().Be(null);
        }

        [Fact]
        public async Task GetAllJobs_Success()
        {
            IContainer container = GetRegistrations();
            IEnumerable<Job> result;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                IJobManager manager = scope.Resolve<IJobManager>();
                await manager.AddJob(new Job()
                {
                    CollectionName = CollectionConstants.ILR2021
                });
                await manager.AddJob(new Job()
                {
                    CollectionName = CollectionConstants.ILR2021
                });
                await manager.AddJob(new Job()
                {
                    CollectionName = CollectionConstants.ILR2021
                });

                result = await manager.GetAllJobs();
            }

            result.Count().Should().Be(3);
        }

        public async Task GetJobByPriority_Ilr_NoJobs()
        {
            IContainer container = GetRegistrations();
            IEnumerable<Job> result;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = container.Resolve<IJobManager>();
                result = await manager.GetJobsByPriorityAsync(100);
            }

            result.Should().BeEmpty();
        }

        public async Task GetJobByPriority_Ilr_submission()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = container.Resolve<IJobManager>();
                await manager.AddJob(new Job
                {
                    Priority = 1,
                    Status = JobStatusType.Ready,
                    CollectionName = CollectionConstants.ILR2021,
                });
                await manager.AddJob(new Job
                {
                    Priority = 2,
                    Status = JobStatusType.Ready,
                    CollectionName = CollectionConstants.ILR2021,
                });

                IEnumerable<Job> result = (await manager.GetJobsByPriorityAsync(100)).ToList();
                result.Should().NotBeEmpty();
                Job job = result.First();
                job.JobId.Should().Be(2);
                job.CollectionName.Should().Be(CollectionConstants.ILR2021);
            }
        }

        [Fact]
        public async Task RemoveJobFromQueue_Fail_ZeroId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.RemoveJobFromQueue(0));
            }
        }

        [Fact]
        public async Task RemoveJobFromQueue_Fail_IdDontExist()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.RemoveJobFromQueue(200));
            }
        }

        [Fact]
        public async Task UpdateJob_Fail_Null()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentNullException>(() => manager.UpdateJob(null));
            }
        }

        [Fact]
        public async Task UpdateJob_Fail_InvalidJobId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.UpdateJob(new Job { JobId = 1000 }));
            }
        }

        [Fact]
        public async Task UpdateJob_Success()
        {
            IContainer container = GetRegistrations();
            SubmittedJob updatedJob;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = scope.Resolve<IJobManager>();
                await manager.AddJob(new Job
                {
                    Status = JobStatusType.Ready,
                    CollectionName = CollectionConstants.ILR2021,
                });

                var queryService = scope.Resolve<IJobQueryService>();

                var job = await queryService.GetJobById(1);
                var newJob = new Job()
                {
                    JobId = job.JobId,
                    Status = JobStatusType.Completed,
                    NotifyEmail = "test@test.com",
                    CreatedBy = "test",
                    CollectionName = CollectionConstants.ILR2021,
                };

                await manager.UpdateJob(newJob);

                updatedJob = await queryService.GetJobById(1);
            }

            updatedJob.Status.Should().Be(JobStatusType.Completed);
            updatedJob.CreatedBy.Should().Be("test");
            updatedJob.NotifyEmail.Should().Be("test@test.com");
        }

        [Fact]
        public async Task UpdateJobStatus_Fail_ZeroId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.UpdateJobStatus(0, JobStatusType.Completed));
            }
        }

        [Fact]
        public async Task UpdateJobStatus_Fail_InvalidJobId()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.UpdateJobStatus(110, JobStatusType.Completed));
            }
        }

        [Fact]
        public async Task UpdateJobStatus_Success_Others()
        {
            IContainer container = GetRegistrations();
            SubmittedJob updatedJob;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, "EAS2021");

                    var manager = scope.Resolve<IJobManager>();
                    await manager.AddJob(new Job
                    {
                        Status = JobStatusType.Ready,
                        CollectionName = "EAS2021",
                    });
                    await manager.UpdateJobStatus(1, JobStatusType.Completed);

                    var queryService = scope.Resolve<IJobQueryService>();
                    updatedJob = await queryService.GetJobById(1);

                    context.IlrJobMetaData.FirstOrDefault(x => x.JobId == 1).Should().BeNull();
                    updatedJob.Status.Should().Be(JobStatusType.Completed);
                }
            }
        }

        [Theory]
        [InlineData(JobStatusType.Failed)]
        [InlineData(JobStatusType.Waiting)]
        [InlineData(JobStatusType.FailedRetry)]
        [InlineData(JobStatusType.Completed)]
        [InlineData(JobStatusType.MovedForProcessing)]
        [InlineData(JobStatusType.Processing)]
        public async Task UpdateJobStatus_Ready(JobStatusType status)
        {
            IContainer container = GetRegistrations();
            SubmittedJob updatedJob;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);

                    var manager = scope.Resolve<IJobManager>();
                    await manager.AddJob(new Job
                    {
                        Status = JobStatusType.Ready,
                        CollectionName = CollectionConstants.ILR2021,
                    });
                    await manager.UpdateJobStatus(1, status);

                    var queryService = scope.Resolve<IJobQueryService>();
                    updatedJob = await queryService.GetJobById(1);

                    updatedJob.Status.Should().Be(status);
                    context.IlrJobMetaData.FirstOrDefault(x => x.JobId == 1).Should().BeNull();
                }
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(JobStatusType.MovedForProcessing)]
        public async Task UpdateJobStatus_Success_EmailSent(JobStatusType? crossLoadingStatus)
        {
            var emailTemplateManager = new Mock<IJobEmailTemplateManager>();
            emailTemplateManager.Setup(x => x.GetTemplate(It.IsAny<long>(), It.IsAny<DateTime>())).Returns(Task.FromResult("template"));

            var emailNotifier = new Mock<IEmailNotifier>();
            emailNotifier.Setup(x => x.SendEmail(It.IsAny<string>(), "test", It.IsAny<Dictionary<string, dynamic>>()));

            IContainer container = GetRegistrations(emailTemplateManager.Object, emailNotifier.Object);

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);
                }

                var manager = scope.Resolve<IJobManager>();
                await manager.AddJob(new Job
                {
                    Status = JobStatusType.Ready,
                    CollectionName = CollectionConstants.ILR2021,
                });

                await manager.UpdateJobStatus(1, JobStatusType.Completed);

                var queryService = scope.Resolve<IJobQueryService>();
                var updatedJob = await queryService.GetJobById(1);
                updatedJob.Status.Should().Be(JobStatusType.Completed);

                emailNotifier.Verify(
                    x => x.SendEmail(It.IsAny<string>(), "template", It.IsAny<Dictionary<string, dynamic>>()), Times.Once());
            }
        }

        [Fact]
        public async Task UpdateCrossLoadingStatus_Success()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context);

                    var manager = scope.Resolve<IJobManager>();
                    await manager.AddJob(new Job
                    {
                        Status = JobStatusType.Ready,
                        CollectionName = CollectionConstants.ILR2021,
                    });
                    await manager.UpdateCrossLoadingStatus(1, JobStatusType.MovedForProcessing, default(CancellationToken));

                    var updatedJob = context.Job.SingleOrDefault(x => x.JobId == 1);
                    updatedJob.CrossLoadingStatus.Should().Be((short)JobStatusType.MovedForProcessing);
                }
            }
        }

        [Fact]
        public async Task CloneJob_Exception_JobIdZero()
        {
            IContainer container = GetRegistrations();
            CancellationToken cancellationToken;

            using (var scope = container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentNullException>(() => manager.CloneJob(0, cancellationToken));
            }
        }

        [Fact]
        public async Task CloneJob_Exception_JobIdNotFound()
        {
            IContainer container = GetRegistrations();
            CancellationToken cancellationToken;

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, CollectionConstants.ILR2021, 1);
                }

                var manager = scope.Resolve<IJobManager>();
                await Assert.ThrowsAsync<ArgumentException>(() => manager.CloneJob(1111, cancellationToken));
            }
        }

        [Fact]
        public async Task CloneJob_NCS_Success()
        {
            IContainer container = GetRegistrations();
            CancellationToken cancellationToken;
            long clonedJobId = 0;
            SubmittedJob newJob;

            var reportEndDate = new DateTime(2018, 10, 10);
            var externalTimeStamp = new DateTime(2018, 11, 11);

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, "NCS1920", 1);
                }

                var manager = scope.Resolve<IJobManager>();
                var ncsJobManager = scope.Resolve<INcsJobManager>();

                var failedJobId = await ncsJobManager.AddJob(
                new NcsJob
                {
                    DssContainer = "Container1",
                    ReportEndDate = reportEndDate,
                    ReportFileName = "report_file",
                    ExternalJobId = "external_job",
                    ExternalTimestamp = externalTimeStamp,
                    Period = 1,
                    TouchpointId = "touchpoint_id",
                    CreatedBy = "test",
                    CollectionName = "NCS1920",
                    Status = JobStatusType.Failed,
                });

                clonedJobId = await manager.CloneJob(failedJobId, cancellationToken);
                var queryService = scope.Resolve<IJobQueryService>();

                newJob = await queryService.GetJobById(clonedJobId);
            }

            clonedJobId.Should().BeGreaterThan(0);
            newJob.CollectionId.Should().Be(1);
            newJob.CollectionName.Should().Be("NCS1920");
            newJob.PeriodNumber.Should().Be(1);
            newJob.Status.Should().Be(JobStatusType.Ready);
            newJob.CreatedBy.Should().Be("ESFA");

            newJob.DssContainer.Should().Be("Container1");
            newJob.ReportEndDate.Should().Be(reportEndDate);
            newJob.ReportFileName.Should().Be("report_file");
            newJob.ExternalJobId.Should().Be("external_job");
            newJob.ExternalTimestamp.Should().Be(externalTimeStamp);
            newJob.TouchpointId.Should().Be("touchpoint_id");
        }

        [Fact]
        public async Task CloneJob_FRM_Success()
        {
            IContainer container = GetRegistrations();
            long clonedJobId = 0;
            CancellationToken cancellationToken;
            SubmittedJob newJob;

            var collectionName = "FRM1920";

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, collectionName, 1);
                }

                var manager = scope.Resolve<IJobManager>();
                var frmReportsJobManager = scope.Resolve<IUpdateJobManager<ReportsPublicationJob>>();

                var failedJobId = await frmReportsJobManager.AddJob(
                new ReportsPublicationJob
                {
                    SourceFolderKey = "source_folder",
                    StorageReference = "storage_reference",
                    CreatedBy = "test",
                    CollectionName = collectionName,
                    Status = JobStatusType.Failed,
                });

                clonedJobId = await manager.CloneJob(failedJobId, cancellationToken);
                var queryService = scope.Resolve<IJobQueryService>();

                newJob = await queryService.GetJobById(clonedJobId);
            }

            clonedJobId.Should().BeGreaterThan(0);
            newJob.CollectionId.Should().Be(1);
            newJob.CollectionName.Should().Be(collectionName);
            newJob.PeriodNumber.Should().Be(1);
            newJob.Status.Should().Be(JobStatusType.Ready);
            newJob.CreatedBy.Should().Be("ESFA");

            newJob.SourceContainerName.Should().Be("frm1819");
            newJob.SourceFolderKey.Should().Be("source_folder");
        }
    }
}
