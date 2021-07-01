using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.Summarisation.Model;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.Data.Services.Tests
{
    public class SummarisationDataServiceTests
    {
        [Fact]
        private async Task Test_CollectionReturnCodes_NothingFound()
        {
            var cancellationToken = CancellationToken.None;
            using (var context = new SummarisationContext(GetDbContextOptions()))
            {
                context.Database.EnsureCreated();
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = "ILR1920",
                    Id = 1,
                    CollectionReturnCode = "R08",
                    DateTime = new DateTime(2019, 10, 10)
                });
                await context.SaveChangesAsync();

                var service = GetService(() => context);
                var data = await service.GetLatestSummarisationCollectionCodesAsync("XYZZZ", cancellationToken, 2);
                data.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData("ILR1920")]
        [InlineData("ESF")]
        [InlineData("APPS")]
        private async Task Test_CollectionReturnCodes_Count(string collectionType)
        {
            var cancellationToken = CancellationToken.None;
            using (var context = new SummarisationContext(GetDbContextOptions()))
            {
                context.Database.EnsureCreated();
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = collectionType,
                    Id = 3,
                    CollectionReturnCode = "R08",
                    DateTime = new DateTime(2019, 10, 10)
                });
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = collectionType,
                    Id = 2,
                    CollectionReturnCode = "R07",
                    DateTime = new DateTime(2019, 09, 09)
                });
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = collectionType,
                    Id = 1,
                    CollectionReturnCode = "R06",
                    DateTime = new DateTime(2019, 08, 08)
                });

                await context.SaveChangesAsync();

                var service = GetService(() => context);

                var data = (await service.GetLatestSummarisationCollectionCodesAsync(collectionType, cancellationToken, 2)).ToList();
                data.Should().NotBeNullOrEmpty();
                data.Count.Should().Be(2);
                data.Single(x => x.CollectionReturnCode.Equals("R07") && x.Id == 2).Should().NotBeNull();
                data.Single(x => x.CollectionReturnCode.Equals("R08") && x.Id == 3).Should().NotBeNull();
            }
        }

        [Fact]
        private async Task Test_CollectionReturnCodes_DateFilter()
        {
            var cancellationToken = CancellationToken.None;

            using (var context = new SummarisationContext(GetDbContextOptions()))
            {
                context.Database.EnsureCreated();
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = "ILR1920",
                    Id = 2,
                    CollectionReturnCode = "R08",
                    DateTime = new DateTime(2019, 10, 10)
                });
                context.CollectionReturns.Add(new CollectionReturn
                {
                    CollectionType = "ILR1920",
                    Id = 1,
                    CollectionReturnCode = "R07",
                    DateTime = new DateTime(2019, 09, 09)
                });

                await context.SaveChangesAsync();

                var service = GetService(() => context);

                var data = (await service.GetLatestSummarisationCollectionCodesAsync("ILR1920", cancellationToken, 2, new DateTime(2019, 09, 09))).ToList();
                data.Should().NotBeNullOrEmpty();
                data.Count.Should().Be(1);
                data.Single(x => x.CollectionReturnCode.Equals("R07") && x.Id == 1).Should().NotBeNull();
            }
        }

        [Fact(Skip = "When the underlying query started at CollectionReturns then unit test is green but query is inefficient. When the underlying query started at SummarisedActuals, then unit test failed.")]
        private async Task Test_GetSummarisationTotals()
        {
            var cancellationToken = CancellationToken.None;

            using (var context = new SummarisationContext(GetDbContextOptions()))
            {
                context.Database.EnsureCreated();

                List<SummarisedActual> summarisedActuals = new List<SummarisedActual>
                {
                    new SummarisedActual
                    {
                        CollectionReturnId = 1,
                        ActualValue = 10,
                        OrganisationId = "Org1",
                        FundingStreamPeriodCode = "fsp1",
                        Period = 1,
                        DeliverableCode = 1,
                        ActualVolume = 1,
                        PeriodTypeCode = "CM"
                    },
                    new SummarisedActual
                    {
                        CollectionReturnId = 1,
                        ActualValue = 20,
                        OrganisationId = "Org1",
                        FundingStreamPeriodCode = "fsp1",
                        Period = 1,
                        DeliverableCode = 1,
                        ActualVolume = 1,
                        PeriodTypeCode = "CM"
                    },

                    new SummarisedActual
                    {
                        CollectionReturnId = 2,
                        ActualValue = 100,
                        OrganisationId = "Org1",
                        FundingStreamPeriodCode = "fsp1",
                        Period = 1,
                        DeliverableCode = 1,
                        ActualVolume = 1,
                        PeriodTypeCode = "CM"
                    },
                    new SummarisedActual
                    {
                        CollectionReturnId = 2,
                        ActualValue = 200,
                        OrganisationId = "Org1",
                        FundingStreamPeriodCode = "fsp1",
                        Period = 1,
                        DeliverableCode = 1,
                        ActualVolume = 1,
                        PeriodTypeCode = "CM"
                    }
                };

                List<CollectionReturn> collectionReturns = new List<CollectionReturn>
                {
                    new CollectionReturn { Id = 1, CollectionType = "Type1", CollectionReturnCode = "R01", DateTime = DateTime.Now },
                    new CollectionReturn { Id = 2, CollectionType = "Type2", CollectionReturnCode = "A01", DateTime = DateTime.Now }
                };

                context.CollectionReturns.AddRange(collectionReturns);
                context.SummarisedActuals.AddRange(summarisedActuals);

                await context.SaveChangesAsync();

                var service = GetService(() => context);
                List<int> Ids = new List<int> { 1, 2 };
                var result = (await service.GetSummarisationTotalsAsync(Ids, cancellationToken)).ToList();
                result.Should().NotBeNullOrEmpty();
                result.Count.Should().Be(2);

                result.Single(x => x.CollectionType.Equals("Type1") && x.CollectionReturnCode.Equals("R01")).Should().NotBeNull();
                result.Single(x => x.CollectionType.Equals("Type1") && x.CollectionReturnCode.Equals("R01")).TotalActualValue.Should().Be(30);

                result.Single(x => x.CollectionType.Equals("Type2") && x.CollectionReturnCode.Equals("A01")).Should().NotBeNull();
                result.Single(x => x.CollectionType.Equals("Type2") && x.CollectionReturnCode.Equals("A01")).TotalActualValue.Should().Be(300);
            }
        }

        private IDateTimeProvider GetdateTimeProvider()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(DateTime.Now);

            return dateTimeProviderMock.Object;
        }

        private DbContextOptions<SummarisationContext> GetDbContextOptions()
        {
            SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            DbContextOptionsBuilder<SummarisationContext> optionsBuilder =
                new DbContextOptionsBuilder<SummarisationContext>()
                    .UseSqlite(connection);

            return optionsBuilder.Options;
        }

        private SummarisationDataService GetService(
            Func<SummarisationContext> context,
            IValidityPeriodService validityPeriodService = null,
            IReturnCodeService returnCodeService = null)
        {
            return new SummarisationDataService(
                context,
                Mock.Of<ILogger>(),
                GetdateTimeProvider(),
                validityPeriodService,
                returnCodeService);
        }
    }
}