using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Controllers;
using ESFA.DC.JobQueueManager.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Job.WebApi.Tests
{
    public class ReturnsCalendarControllerTests
    {
        [Fact]
        public async Task GetAllPeriodsAsync()
        {
            var retCalServiceMock = new Mock<IReturnCalendarService>();
            var data = new List<ReturnPeriod>()
            {
                new ReturnPeriod()
                {
                    PeriodNumber = 1,
                    CollectionId = 10,
                    CalendarMonth = 8,
                    CalendarYear = 2020,
                    CollectionName = "test",
                    CollectionYear = 1920,
                    StartDateTimeUtc = new DateTime(2020, 01, 01),
                    EndDateTimeUtc = new DateTime(2020, 01, 31),
                    ReturnPeriodId = 1,
                },
                new ReturnPeriod()
                {
                    PeriodNumber = 2,
                    CollectionId = 11,
                    CalendarMonth = 8,
                    CalendarYear = 2020,
                    CollectionName = "test2",
                    CollectionYear = 1920,
                    StartDateTimeUtc = new DateTime(2020, 02, 01),
                    EndDateTimeUtc = new DateTime(2020, 02, 28),
                    ReturnPeriodId = 2,
                },
            };
            var dataArray = data.ToArray();

            retCalServiceMock.Setup(x => x.GetAllPeriodsAsync(It.IsAny<string>(), null)).ReturnsAsync(() => dataArray);
            var rcController = new ReturnsCalendarController(retCalServiceMock.Object);
            var result = await rcController.GetAllPeriodsAsync("test");
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(data);
        }
    }
}
