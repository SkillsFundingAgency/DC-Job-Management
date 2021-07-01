using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PeriodEndRepositoryTests
    {
        [Fact]
        public async Task GetPeriodEndHistoryDetailsAsync_ReturnsUKDates()
        {
            var collectionName = "ILR2021";
            var startDate = new DateTime(2020, 9, 30, 9, 0, 0);
            var endDate = new DateTime(2020, 9, 30, 17, 0, 0);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(m => m.ConvertUtcToUk(startDate)).Returns(startDate.AddHours(-1));
            dateTimeProviderMock.Setup(m => m.ConvertUtcToUk(endDate)).Returns(endDate.AddHours(-1));

            var periodEnds = new List<JobQueueManager.Data.Entities.PeriodEnd>
            {
                new JobQueueManager.Data.Entities.PeriodEnd
                {
                    PeriodEndId = 1,
                    PeriodEndStarted = startDate,
                    PeriodEndFinished = endDate,
                    Period = new ReturnPeriod
                    {
                        PeriodNumber = 2,
                        Collection = new Collection
                        {
                            Name = collectionName,
                            CollectionYear = 2021
                        }
                    }
                }
            };

            var periodEndMock = periodEnds.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(m => m.PeriodEnd).Returns(periodEndMock.Object);

            Func<IJobQueueDataContext> factory = () => contextMock.Object;

            var service = GetService(dateTimeProviderMock.Object, factory);
            var result = await service.GetPeriodEndHistoryDetailsAsync(collectionName, CancellationToken.None);

            result.Should().HaveCount(1);

            var periodEnd = result.First();
            periodEnd.PeriodEndStart.Should().Be(startDate.AddHours(-1));
            periodEnd.PeriodEndFinish.Should().Be(endDate.AddHours(-1));
            periodEnd.PeriodEndId.Should().Be(1);
        }

        private IPeriodEndRepository GetService(
            IDateTimeProvider dateTimeProvider = null,
            Func<IJobQueueDataContext> contextFactory = null,
            PeriodEndJobSettings periodEndJobSettings = null,
            IAuditFactory auditfactory = null)
        {
            return new PeriodEndRepository(Mock.Of<ILogger>(), dateTimeProvider, contextFactory, periodEndJobSettings, auditfactory);
        }
    }
}