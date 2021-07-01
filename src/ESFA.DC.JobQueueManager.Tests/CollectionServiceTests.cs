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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class CollectionServiceTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task GetAcademicYears_Test_OneYear()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = (await service.GetAcademicYearsAsync(CancellationToken.None, new DateTime(2019, 8, 3), true)).ToList();
                data.Count.Should().Be(1);
                data.First().Should().Be(1819);
            }
        }

        [Fact]
        public async Task GetAcademicYears_Test_Multi()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = (await service.GetAcademicYearsAsync(CancellationToken.None, new DateTime(2019, 8, 4), true)).ToList();
                data.Count.Should().Be(2);
                data.First().Should().Be(1920);
            }
        }

        [Fact]
        public async Task GetAvailableCollectionYears_Returns_Correct_Number()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetAvailableCollectionYearsAsync(CancellationToken.None);
                data.Count().Should().Be(4);
            }
        }

        [Fact]
        public async Task GetCollectionByDate_Returns_Open_Collection()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionByDateAsync(CancellationToken.None, "NCS", new DateTime(2020, 8, 10));
                data.Should().NotBeNull();
                data.CollectionId.Should().Be(3);
            }
        }

        [Fact]
        public async Task GetCollectionByDate_Returns_Next_Open_Collection()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionByDateAsync(CancellationToken.None, "NCS", new DateTime(2020, 7, 1));
                data.Should().NotBeNull();
                data.CollectionId.Should().Be(3);
            }
        }

        [Fact]
        public async Task GetCollectionsByYearAsync_Returns_Correct_Number()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionsByYearAsync(CancellationToken.None, 1920);
                data.Count().Should().Be(1);
            }
        }

        [Fact]
        public async Task GetCollectionsByYearAsync_Returns_Correct_StartAndEndDate()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionsByYearAsync(CancellationToken.None, 1920);
                var collection = data.First();

                collection.StartDateTimeUtc.Should().Be(new DateTime(2019, 8, 4));
                collection.EndDateTimeUtc.Should().Be(new DateTime(2019, 10, 3));
            }
        }

        [Fact]
        public async Task GetCollectionsByDateRangeAsync_Returns_AllCollections_In_Date_Range()
        {
            IContainer container = GetRegistrations();

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetOpenCollectionsByDateRangeAsync(new DateTime(2018, 8, 4), new DateTime(2018, 9, 5), CancellationToken.None);
                data.Count().Should().Be(1);
            }
        }

        [Fact]
        public async Task GetCollectionStartDateAsync_Returns_ValidStartDateTime()
        {
            IContainer container = GetRegistrations();
            var startDateTimeUtc = new DateTime(2021, 8, 1);

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionStartDateAsync("EAS2122", CancellationToken.None);
                data.Should().Equals(startDateTimeUtc);
            }
        }

        [Fact]
        public async Task GetCollectionStartDateAsync_Returns_NullStartDateTime()
        {
            IContainer container = GetRegistrations();
            DateTime? startDateTimeUtc = null;

            using (var scope = container.BeginLifetimeScope())
            {
                SetupData(scope);
                var service = scope.Resolve<ICollectionService>();
                var data = await service.GetCollectionStartDateAsync("EAS2122NULL", CancellationToken.None);
                data.Should().Equals(startDateTimeUtc);
            }
        }

        private void SetupData(ILifetimeScope scope)
        {
            var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
            using (var context = new JobQueueDataContext(options))
            {
                SetupDatabaseForJobs(context);

                var ilrCollectionType = new CollectionType()
                {
                    CollectionTypeId = 1,
                    Description = "submission",
                    Type = "ILR",
                };

                var ncsCollectionType = new CollectionType()
                {
                    CollectionTypeId = 5,
                    Description = "submission",
                    Type = "NCS",
                };

                context.CollectionType.Add(ilrCollectionType);
                context.CollectionType.Add(ncsCollectionType);

                var collection1 = new Collection()
                {
                    CollectionId = 1,
                    CollectionTypeId = 1,
                    Name = CollectionConstants.ILR1819,
                    CollectionYear = 1819,
                    ReturnPeriod = new List<ReturnPeriod>()
                    {
                        new ReturnPeriod()
                        {
                            PeriodNumber = 1,
                            StartDateTimeUtc = new DateTime(2018, 8, 4),
                            EndDateTimeUtc = new DateTime(2018, 9, 3),
                        },
                        new ReturnPeriod()
                        {
                            PeriodNumber = 2,
                            StartDateTimeUtc = new DateTime(2018, 9, 4),
                            EndDateTimeUtc = new DateTime(2018, 10, 3),
                        },
                    },
                    CollectionType = ilrCollectionType,
                };

                var collection2 = new Collection()
                {
                    CollectionId = 2,
                    CollectionTypeId = 1,
                    Name = CollectionConstants.ILR2021,
                    CollectionYear = 1920,
                    ReturnPeriod = new List<ReturnPeriod>()
                    {
                        new ReturnPeriod()
                        {
                            PeriodNumber = 1,
                            StartDateTimeUtc = new DateTime(2019, 8, 4),
                            EndDateTimeUtc = new DateTime(2019, 9, 3),
                        },
                        new ReturnPeriod()
                        {
                            PeriodNumber = 2,
                            StartDateTimeUtc = new DateTime(2019, 9, 4),
                            EndDateTimeUtc = new DateTime(2019, 10, 3),
                        },
                    },
                    CollectionType = ilrCollectionType,
                };

                var collection3 = new Collection()
                {
                    CollectionId = 3,
                    CollectionTypeId = 5,
                    Name = "NCS1",
                    CollectionYear = 2021,
                    ReturnPeriod = new List<ReturnPeriod>()
                    {
                        new ReturnPeriod()
                        {
                            PeriodNumber = 1,
                            StartDateTimeUtc = new DateTime(2020, 8, 4),
                            EndDateTimeUtc = new DateTime(2020, 9, 3),
                        },
                        new ReturnPeriod()
                        {
                            PeriodNumber = 2,
                            StartDateTimeUtc = new DateTime(2020, 9, 4),
                            EndDateTimeUtc = new DateTime(2020, 10, 3),
                        },
                    },
                    CollectionType = ncsCollectionType,
                };

                var collection4 = new Collection()
                {
                    CollectionId = 4,
                    CollectionTypeId = 1,
                    Name = "EAS2122",
                    CollectionYear = 2122,
                    ReturnPeriod = new List<ReturnPeriod>()
                    {
                        new ReturnPeriod()
                        {
                            PeriodNumber = 1,
                            StartDateTimeUtc = new DateTime(2021, 8, 1),
                            EndDateTimeUtc = new DateTime(2020, 9, 3),
                        },
                        new ReturnPeriod()
                        {
                            PeriodNumber = 2,
                            StartDateTimeUtc = new DateTime(2021, 9, 4),
                            EndDateTimeUtc = new DateTime(2021, 10, 3),
                        },
                    },
                    CollectionType = ncsCollectionType,
                };

                var collection5 = new Collection()
                {
                    CollectionId = 5,
                    CollectionTypeId = 1,
                    Name = "EAS2122NULL",
                    CollectionYear = 2122,
                    CollectionType = ncsCollectionType,
                };

                context.Add(collection1);
                context.Add(collection2);
                context.Add(collection3);
                context.Add(collection4);
                context.Add(collection5);
                context.SaveChanges();
            }
        }
    }
}
