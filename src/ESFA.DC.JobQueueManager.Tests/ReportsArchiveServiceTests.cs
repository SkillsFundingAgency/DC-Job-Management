using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class ReportsArchiveServiceTests
    {
        [Fact]
        public async Task Test_Empty()
        {
            IContainer container = GetRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();
                }

                var service = scope.Resolve<IReportsArchiveService>();
                var result = await service.GetAllReportsArchivesAsync(CancellationToken.None, 1000000);
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Test_Not_Empty()
        {
            IContainer container = GetRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();
                    var collectionType = new CollectionType()
                    {
                        Type = "ILR",
                        CollectionTypeId = 1,
                        ConcurrentExecutionCount = 10,
                        Description = "ILR Type",
                    };
                    context.ReportsArchive.Add(new ReportsArchive()
                    {
                        InSld = true,
                        CollectionTypeId = 1,
                        CollectionType = collectionType,
                        Year = 1920,
                        Period = 1,
                        UploadedDateTimeUtc = DateTime.UtcNow.AddDays(-1),
                        UploadedBy = "Test user",
                        FileName = "ILR-Reports.zip",
                        Ukprn = 10000
                    });
                    context.ReportsArchive.Add(new ReportsArchive()
                    {
                        InSld = true,
                        CollectionTypeId = 1,
                        CollectionType = collectionType,
                        Year = 1819,
                        Period = 2,
                        UploadedDateTimeUtc = DateTime.UtcNow.AddDays(-2),
                        UploadedBy = "Test user",
                        Ukprn = 10000,
                        FileName = "ILR-Reports2.zip",
                    });
                    context.ReportsArchive.Add(new ReportsArchive()
                    {
                        InSld = false,
                        CollectionTypeId = 1,
                        CollectionType = collectionType,
                        Year = 1819,
                        Period = 3,
                        UploadedDateTimeUtc = DateTime.UtcNow.AddDays(-2),
                        UploadedBy = "Test user",
                        Ukprn = 10000,
                        FileName = "ILR-Reports3.zip",
                    });

                    await context.SaveChangesAsync(CancellationToken.None);
                }

                var service = scope.Resolve<IReportsArchiveService>();
                var result = await service.GetAllReportsArchivesAsync(CancellationToken.None, 10000);
                result.Should().NotBeNullOrEmpty();
                result.Count.Should().Be(2);

                result.Any(x => x.InSld && x.Period == 1 && x.Year == 1920 && x.CollectionType == "ILR" && x.FileName == "ILR-Reports.zip").Should().BeTrue();
                result.Any(x => x.InSld && x.Period == 2 && x.Year == 1819 && x.CollectionType == "ILR" && x.FileName == "ILR-Reports2.zip").Should().BeTrue();
                result.Any(x => x.Period == 3 && x.Year == 1819 && x.CollectionType == "ILR").Should().BeFalse();
            }
        }

        [Fact]
        public async Task Test_ProvidersExist()
        {
            IContainer container = GetRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();
                    var collectionType = new CollectionType()
                    {
                        Type = "ILR",
                        CollectionTypeId = 1,
                        ConcurrentExecutionCount = 10,
                        Description = "ILR Type",
                    };
                    context.ReportsArchive.Add(CreateArchiveEntity(collectionType, 10002));
                    context.ReportsArchive.Add(CreateArchiveEntity(collectionType, 10001));
                    await context.SaveChangesAsync(CancellationToken.None);
                }

                var service = scope.Resolve<IReportsArchiveService>();
                var input = new List<long>()
                {
                    10001,
                    10002,
                    99999
                };

                var result = await service.GetProvidersWithDataAsync(CancellationToken.None, input);
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);
                result.Any(x => x == 10001).Should().BeTrue();
                result.Any(x => x == 10002).Should().BeTrue();
                result.Any(x => x == 99999).Should().BeFalse();
            }
        }

        protected IContainer GetRegistrations()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<ReportsArchiveService>().As<IReportsArchiveService>();
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.Register(context =>
                {
                    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    DbContextOptionsBuilder<JobQueueDataContext> optionsBuilder =
                        new DbContextOptionsBuilder<JobQueueDataContext>()
                            .UseSqlite(connection);

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            IContainer container = builder.Build();
            return container;
        }

        private ReportsArchive CreateArchiveEntity(CollectionType collectionType, long ukprn)
        {
            return new ReportsArchive()
            {
                InSld = false,
                CollectionTypeId = 1,
                CollectionType = collectionType,
                Year = 1920,
                Period = 1,
                UploadedDateTimeUtc = DateTime.UtcNow.AddDays(-1),
                UploadedBy = "Test user",
                FileName = "ILR-Reports.zip",
                Ukprn = ukprn
            };
        }
    }
}
