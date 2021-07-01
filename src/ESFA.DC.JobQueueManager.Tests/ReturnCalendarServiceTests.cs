using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class ReturnCalendarServiceTests
    {
        [Theory]
        [InlineData("ILR")]
        [InlineData("EAS")]
        [InlineData("ESF")]
        public async Task GetOpenPeriodsAsync_Test(string collectionTypeName)
        {
            IContainer container = GetRegistrations(DateTime.UtcNow);

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var collectionType = new CollectionType
                    {
                        CollectionTypeId = 1,
                        Description = "submission",
                        Type = collectionTypeName
                    };

                    context.CollectionType.Add(collectionType);

                    context.Collection.Add(new Collection
                    {
                        CollectionId = 1,
                        CollectionType = collectionType,
                        Name = $"{collectionTypeName}1920",
                        CollectionYear = 1920
                    });
                    context.Collection.Add(new Collection
                    {
                        CollectionId = 2,
                        CollectionType = collectionType,
                        Name = $"{collectionTypeName}2021",
                        CollectionYear = 2021
                    });

                    var dateTimeProvider = scope.Resolve<IDateTimeProvider>();
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 13,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(-3),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(1),
                        CollectionId = 1
                    });
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 1,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(-2),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddMinutes(1),
                        CollectionId = 2
                    });

                    // This is outside Now datetime
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 2,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddMinutes(1),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(10),
                        CollectionId = 2
                    });

                    await context.SaveChangesAsync();
                }

                var returnCalendarService = scope.Resolve<IReturnCalendarService>();
                var data = await returnCalendarService.GetOpenPeriodsAsync(null, collectionTypeName);
                data.Should().NotBeNull();
                data.Count.Should().Be(2);

                data.SingleOrDefault(x => x.PeriodNumber == 13 && x.CollectionName == $"{collectionTypeName}1920").Should().NotBeNull();
                data.SingleOrDefault(x => x.PeriodNumber == 1 && x.CollectionName == $"{collectionTypeName}2021").Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("ILR")]
        [InlineData("EAS")]
        [InlineData("ESF")]
        public async Task GetRecentlyClosedPeriodAsync_Test(string collectionTypeName)
        {
            IContainer container = GetRegistrations(DateTime.UtcNow);

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var collectionType = new CollectionType
                    {
                        CollectionTypeId = 1,
                        Description = "submission",
                        Type = collectionTypeName
                    };

                    context.CollectionType.Add(collectionType);

                    context.Collection.Add(new Collection
                    {
                        CollectionId = 1,
                        CollectionType = collectionType,
                        Name = $"{collectionTypeName}1920",
                        CollectionYear = 1920
                    });
                    context.Collection.Add(new Collection
                    {
                        CollectionId = 2,
                        CollectionType = collectionType,
                        Name = $"{collectionTypeName}2021",
                        CollectionYear = 2021
                    });

                    var dateTimeProvider = scope.Resolve<IDateTimeProvider>();
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 13,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(-3),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(-1),
                        CollectionId = 1
                    });
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 1,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(-2),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddMinutes(-1),
                        CollectionId = 2
                    });

                    // This is outside Now datetime
                    context.Add(new ReturnPeriod
                    {
                        PeriodNumber = 2,
                        StartDateTimeUtc = dateTimeProvider.GetNowUtc().AddMinutes(1),
                        EndDateTimeUtc = dateTimeProvider.GetNowUtc().AddDays(10),
                        CollectionId = 2
                    });

                    await context.SaveChangesAsync();
                }

                var returnCalendarService = scope.Resolve<IReturnCalendarService>();
                var data = await returnCalendarService.GetRecentlyClosedPeriodAsync(null, collectionTypeName);
                data.Should().NotBeNull();

                data.PeriodNumber.Should().Be(1);
                data.CollectionName.Should().Be($"{collectionTypeName}2021");
            }
        }

        [Theory]
        [InlineData(2, 2, 2, false)]
        [InlineData(2, 5, 2, false)]
        public async Task GetPeriodEndPeriod_Test(int month, int day, int expectedPeriod, bool expectedClosed)
        {
            IContainer container = GetRegistrations(new DateTime(2018, month, day));

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    await context.Database.EnsureCreatedAsync();

                    var collectionType = new CollectionType
                    {
                        CollectionTypeId = 1,
                        Description = "submission",
                        Type = "ILR"
                    };

                    context.CollectionType.Add(collectionType);

                    var collection = new Collection
                    {
                        CollectionType = collectionType,
                        Name = CollectionConstants.ILR1819,
                        CollectionYear = 1819
                    };

                    context.Collection.Add(collection);

                    var p1 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2018, 1, 1),
                        EndDateTimeUtc = new DateTime(2018, 2, 1),
                        Collection = collection,
                        PeriodNumber = 1,
                        CalendarMonth = 1,
                        CalendarYear = 2019
                    };

                    context.ReturnPeriod.Add(p1);

                    var p2 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2018, 2, 3),
                        EndDateTimeUtc = new DateTime(2018, 3, 1),
                        Collection = collection,
                        PeriodNumber = 2,
                        CalendarMonth = 2,
                        CalendarYear = 2019
                    };

                    context.ReturnPeriod.Add(p2);

                    var p3 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2018, 3, 3),
                        EndDateTimeUtc = new DateTime(2018, 4, 1),
                        Collection = collection,
                        PeriodNumber = 3,
                        CalendarMonth = 3,
                        CalendarYear = 2019
                    };

                    context.ReturnPeriod.Add(p3);

                    context.PeriodEnd.Add(new Data.Entities.PeriodEnd
                    {
                        Period = p1,
                        Closed = true
                    });

                    await context.SaveChangesAsync();
                }

                var returnCalendarService = scope.Resolve<IReturnCalendarService>();
                var data = await returnCalendarService.GetPeriodEndPeriod(CollectionTypeConstants.Ilr);
                data.Should().NotBeNull();

                data.Period.Should().Be(expectedPeriod);
                data.PeriodClosed.Should().Be(expectedClosed);
                data.Year.Should().Be(1819);
            }
        }

        [Theory]
        [InlineData(9, 2, 1)]
        [InlineData(8, 9, 1)]
        [InlineData(8, 5, 12)]
        [InlineData(9, 6, 13)]
        public async Task GetNextClosingPeriod_Test(int month, int day, int expectedPeriod)
        {
            IContainer container = GetRegistrations(new DateTime(2021, month, day));

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    await context.Database.EnsureCreatedAsync();

                    var collectionType = new CollectionType
                    {
                        CollectionTypeId = 1,
                        Description = "submission",
                        Type = "ILR"
                    };

                    context.CollectionType.Add(collectionType);

                    var collection1 = new Collection
                    {
                        CollectionType = collectionType,
                        Name = CollectionConstants.ILR1920,
                        CollectionYear = 1920,
                        CollectionId = 1
                    };

                    var collection2 = new Collection
                    {
                        CollectionType = collectionType,
                        Name = CollectionConstants.ILR2021,
                        CollectionYear = 2021,
                        CollectionId = 2
                    };

                    context.Collection.Add(collection1);
                    context.Collection.Add(collection2);

                    var p1 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2021, 7, 10),
                        EndDateTimeUtc = new DateTime(2021, 8, 6),
                        Collection = collection1,
                        PeriodNumber = 12,
                        CalendarMonth = 7,
                        CalendarYear = 2021
                    };

                    context.ReturnPeriod.Add(p1);

                    var p2 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2021, 8, 8),
                        EndDateTimeUtc = new DateTime(2021, 9, 14),
                        Collection = collection1,
                        PeriodNumber = 13,
                        CalendarMonth = 8,
                        CalendarYear = 2021
                    };

                    context.ReturnPeriod.Add(p2);

                    var p3 = new ReturnPeriod
                    {
                        StartDateTimeUtc = new DateTime(2021, 8, 10),
                        EndDateTimeUtc = new DateTime(2021, 9, 5),
                        Collection = collection2,
                        PeriodNumber = 1,
                        CalendarMonth = 8,
                        CalendarYear = 2021
                    };

                    context.ReturnPeriod.Add(p3);

                    await context.SaveChangesAsync();
                }

                var returnCalendarService = scope.Resolve<IReturnCalendarService>();
                var data = await returnCalendarService.GetNextClosingPeriodAsync(CollectionTypeConstants.Ilr, CancellationToken.None);
                data.Should().NotBeNull();

                data.PeriodNumber.Should().Be(expectedPeriod);
            }
        }

        protected IContainer GetRegistrations(DateTime now)
        {
            ContainerBuilder builder = new ContainerBuilder();

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(now);

            builder.RegisterInstance(dateTimeProviderMock.Object).As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>().InstancePerLifetimeScope();

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
    }
}
