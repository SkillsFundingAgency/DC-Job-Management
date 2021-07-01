// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autofac;
// using ESFA.DC.DateTimeProvider.Interface;
// using ESFA.DC.JobNotifications.Interfaces;
// using ESFA.DC.JobQueueManager.Data;
// using ESFA.DC.JobQueueManager.Interfaces;
// using ESFA.DC.Jobs.Model;
// using ESFA.DC.Jobs.Model.Enums;
// using ESFA.DC.Logging.Interfaces;
// using FluentAssertions;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Xunit;
// using JobType = ESFA.DC.Jobs.Model.JobType;
// using JobTypeGroup = ESFA.DC.JobQueueManager.Data.Entities.JobTypeGroup;

// namespace ESFA.DC.JobQueueManager.Tests
// {
//    public class NcsJobManagerTests
//    {
//        [Fact]
//        public async Task AddJob_Null()
//        {
//            IContainer container = GetRegistrations();

// using (var scope = container.BeginLifetimeScope())
//            {
//                INcsJobManager manager = scope.Resolve<INcsJobManager>();
//                await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddJob(null));
//            }
//        }

// [Fact]
//        public async Task AddJob_Success()
//        {
//            IContainer container = GetRegistrations();

// using (var scope = container.BeginLifetimeScope())
//            {
//                // Create the schema in the database
//                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
//                using (var context = new JobQueueDataContext(options))
//                {
//                    context.Database.EnsureCreated();
//                    context.JobTypeGroup.Add(new JobTypeGroup()
//                    {
//                        JobTypeGroupId = 1,
//                        Description = "submission"
//                    });
//                    context.JobType.Add(new Data.Entities.JobType()
//                    {
//                        JobTypeId = (short)EnumJobType.NcsSubmission,
//                        JobTypeGroupId = 1,
//                        Description = "ncs",
//                        Title = "ncs"
//                    });
//                    context.SaveChanges();
//                }

// INcsJobManager manager = scope.Resolve<INcsJobManager>();

// NcsJob job = new NcsJob()
//                {
//                    JobId = 1,
//                    ExternalJobId = "2",
//                    TouchpointId = "Touchpoint",
//                    ExternalTimestamp = new DateTime(2019, 01, 01),
//                    Ukprn = 3,
//                    ContainerName = "ContainerName",
//                    ReportFileName = "ReportFileName",
//                    CollectionName = "NCS",
//                    PeriodNumber = 8,
//                    CollectionYear = 2018,
//                    JobType = EnumJobType.NcsSubmission
//                };

// var result = await manager.AddJob(job);

// result.Should().Be(1);
//            }
//        }

// [Fact]
//        public async Task GetJobById_NotFound()
//        {
//            IContainer container = GetRegistrations();

// using (var scope = container.BeginLifetimeScope())
//            {
//                // Create the schema in the database
//                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
//                using (var context = new JobQueueDataContext(options))
//                {
//                    context.Database.EnsureCreated();
//                }

// INcsJobManager manager = scope.Resolve<INcsJobManager>();
//                await Assert.ThrowsAsync<ArgumentException>(() => manager.GetJobById(100));
//            }
//        }

// [Fact]
//        public async Task GetJobById_Successful()
//        {
//            IContainer container = GetRegistrations();
//            NcsJob savedJob;
//            var externalTimeStamp = new DateTime(2019, 01, 01);

// using (var scope = container.BeginLifetimeScope())
//            {
//                // Create the schema in the database
//                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
//                using (var context = new JobQueueDataContext(options))
//                {
//                    context.Database.EnsureCreated();
//                    context.JobTypeGroup.Add(new JobTypeGroup()
//                    {
//                        JobTypeGroupId = 1,
//                        Description = "submission"
//                    });
//                    context.JobType.Add(new Data.Entities.JobType()
//                    {
//                        JobTypeId = (short)EnumJobType.NcsSubmission,
//                        JobTypeGroupId = 1,
//                        Description = "ncs",
//                        Title = "ncs"
//                    });
//                    context.SaveChanges();
//                }

// INcsJobManager manager = scope.Resolve<INcsJobManager>();

// NcsJob job = new NcsJob()
//                {
//                    JobId = 1,
//                    ExternalJobId = "2",
//                    TouchpointId = "Touchpoint",
//                    ExternalTimestamp = externalTimeStamp,
//                    Ukprn = 3,
//                    ContainerName = "ContainerName",
//                    ReportFileName = "ReportFileName",
//                    CollectionName = "NCS",
//                    PeriodNumber = 8,
//                    CollectionYear = 2018,
//                    JobType = EnumJobType.NcsSubmission
//                };

// await manager.AddJob(job);

// savedJob = await manager.GetJobById(1);
//            }

// savedJob.Should().NotBeNull();
//            savedJob.JobId.Should().Be(1);
//            savedJob.ExternalJobId.Should().Be("2");
//            savedJob.TouchpointId.Should().Be("Touchpoint");
//            savedJob.ExternalTimestamp.Should().Be(externalTimeStamp);
//            savedJob.Ukprn.Should().Be(3);
//            savedJob.ContainerName.Should().Be("ContainerName");
//            savedJob.ReportFileName.Should().Be("ReportFileName");
//            savedJob.CollectionName.Should().Be("NCS");
//            savedJob.PeriodNumber.Should().Be(8);
//            savedJob.CollectionYear.Should().Be(2018);
//        }

// [Fact]
//        public async Task GetAllJobs_Successful()
//        {
//            IContainer container = GetRegistrations();
//            List<NcsJob> result;

// using (var scope = container.BeginLifetimeScope())
//            {
//                // Create the schema in the database
//                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
//                using (var context = new JobQueueDataContext(options))
//                {
//                    context.Database.EnsureCreated();
//                    context.JobTypeGroup.Add(new JobTypeGroup()
//                    {
//                        JobTypeGroupId = 1,
//                        Description = "submission"
//                    });
//                    context.JobType.Add(new Data.Entities.JobType()
//                    {
//                        JobTypeId = (short)EnumJobType.NcsSubmission,
//                        JobTypeGroupId = 1,
//                        Description = "ncs",
//                        Title = "ncs"
//                    });
//                    context.SaveChanges();
//                }

// INcsJobManager manager = scope.Resolve<INcsJobManager>();
//                await manager.AddJob(new NcsJob()
//                {
//                    JobId = 1,
//                    ExternalJobId = "2",
//                    TouchpointId = "Touchpoint",
//                    ExternalTimestamp = new DateTime(2019, 01, 01),
//                    Ukprn = 1,
//                    ContainerName = "ContainerName",
//                    ReportFileName = "ReportFileName",
//                    CollectionName = "NCS",
//                    PeriodNumber = 8,
//                    CollectionYear = 2018,
//                    JobType = EnumJobType.NcsSubmission
//                });
//                await manager.AddJob(new NcsJob()
//                {
//                    JobId = 2,
//                    ExternalJobId = "3",
//                    TouchpointId = "Touchpoint",
//                    ExternalTimestamp = new DateTime(2019, 01, 01),
//                    Ukprn = 2,
//                    ContainerName = "ContainerName",
//                    ReportFileName = "ReportFileName",
//                    CollectionName = "NCS",
//                    PeriodNumber = 8,
//                    CollectionYear = 2018,
//                    JobType = EnumJobType.NcsSubmission
//                });
//                await manager.AddJob(new NcsJob()
//                {
//                    JobId = 3,
//                    ExternalJobId = "4",
//                    TouchpointId = "Touchpoint",
//                    ExternalTimestamp = new DateTime(2019, 01, 01),
//                    Ukprn = 3,
//                    ContainerName = "ContainerName",
//                    ReportFileName = "ReportFileName",
//                    CollectionName = "NCS",
//                    PeriodNumber = 8,
//                    CollectionYear = 2018,
//                    JobType = EnumJobType.NcsSubmission
//                });

// result = (await manager.GetAllJobs()).ToList();
//            }

// result.Should().NotBeNull();
//            result.Count.Should().Be(3);
//        }

// private IContainer GetRegistrations()
//        {
//            ContainerBuilder builder = new ContainerBuilder();

// var dateTimeProviderMock = new Mock<IDateTimeProvider>();
//            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

// builder.RegisterInstance(dateTimeProviderMock.Object).As<IDateTimeProvider>().SingleInstance();
//            builder.RegisterInstance(new Mock<IEmailNotifier>().Object).As<IEmailNotifier>().SingleInstance();
//            builder.RegisterInstance(new Mock<INcsJobManager>().Object).As<INcsJobManager>().SingleInstance();
//            builder.RegisterInstance(new Mock<IJobEmailTemplateManager>().Object).As<IJobEmailTemplateManager>().SingleInstance();
//            builder.RegisterInstance(new Mock<ILogger>().Object).As<ILogger>().SingleInstance();
//            builder.RegisterInstance(new Mock<IReturnCalendarService>().Object).As<IReturnCalendarService>().SingleInstance();
//            builder.RegisterType<NcsJobManager>().As<INcsJobManager>().InstancePerLifetimeScope();
//            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().InstancePerDependency();

// builder.Register(context =>
//                {
//                    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
//                    connection.Open();
//                    DbContextOptionsBuilder<JobQueueDataContext> optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>()
//                        .UseSqlite(connection);
//                    return optionsBuilder.Options;
//                })
//                .As<DbContextOptions<JobQueueDataContext>>()
//                .SingleInstance();

// IContainer container = builder.Build();
//            return container;
//        }
//    }
// }
