using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.DataAccess;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PeriodEndServiceTests : BaseTest
    {
        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ProceedAsync_NoSubPaths()
        {
            var pathId = 0;
            var pathIds = new List<int> { pathId };
            var year = 1819;
            var period = 1;

            var loggerMock = new Mock<ILogger>();

            var matchedPathController = new Mock<ICriticalPathController>();
            matchedPathController.Setup(c => c.IsMatch(pathId)).Returns(true);
            matchedPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(true);
            matchedPathController.Setup(c => c.Execute(year, period)).ReturnsAsync(new List<int>());

            var notMatchedPathController = new Mock<ICriticalPathController>();
            notMatchedPathController.Setup(c => c.IsMatch(pathId)).Returns(false);
            notMatchedPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(true);

            var pathControllers = new List<ICriticalPathController>
            {
                matchedPathController.Object,
                notMatchedPathController.Object
            };

            var scheduleRepo = new Mock<IScheduleRepository>();

            var dasEnd = new Mock<IDASStoppingPathItem>();

            var paramFactory = new Mock<IPathItemParamsFactory>();

            var validityPeriod = new Mock<IValidityPeriodService>();
            validityPeriod.Setup(c => c.GetValidPeriodsForCollections(year)).ReturnsAsync(BuildValidPeriods(year));

            var returnCalendarService = new Mock<IReturnCalendarService>();

            var failedJobsQueryService = new Mock<IFailedJobQueryServiceILR>();

            var dateTimeProvider = new Mock<IDateTimeProvider>();

            var pathStructureService = new Mock<IPathStructureServiceILR>();

            var dasPaymentsService = new Mock<IDASPaymentsService>();

            var jobQueryService = new Mock<IJobQueryService>();

            var periodEndService = new PeriodEndServiceILR(loggerMock.Object, pathControllers, null, null, null, scheduleRepo.Object, dasEnd.Object, paramFactory.Object, validityPeriod.Object, returnCalendarService.Object, failedJobsQueryService.Object, dateTimeProvider.Object, pathStructureService.Object, dasPaymentsService.Object, jobQueryService.Object);

            await periodEndService.ProceedAsync(pathIds, year, period, CancellationToken.None);

            matchedPathController.Verify(c => c.Execute(year, period), Times.Once);
            notMatchedPathController.Verify(c => c.Execute(year, period), Times.Never);
        }

        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ProceedAsync_SubPathCalled()
        {
            var pathId = 0;
            var pathIds = new List<int> { pathId };

            var subPathId = 1;
            var subPathIds = new List<int> { subPathId };

            var year = 1819;
            var period = 1;

            var loggerMock = new Mock<ILogger>();

            var matchedPathController = new Mock<ICriticalPathController>();
            matchedPathController.Setup(c => c.IsMatch(pathId)).Returns(true);
            matchedPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(true);
            matchedPathController.Setup(c => c.Execute(year, period)).ReturnsAsync(subPathIds);

            var subPathController = new Mock<ICriticalPathController>();
            subPathController.Setup(c => c.IsMatch(subPathId)).Returns(true);
            subPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(true);

            var pathControllers = new List<ICriticalPathController>
            {
                matchedPathController.Object,
                subPathController.Object
            };

            var scheduleRepo = new Mock<IScheduleRepository>();

            var dasEnd = new Mock<IDASStoppingPathItem>();

            var paramFactory = new Mock<IPathItemParamsFactory>();

            var validityPeriod = new Mock<IValidityPeriodService>();
            validityPeriod.Setup(c => c.GetValidPeriodsForCollections(year)).ReturnsAsync(BuildValidPeriods(year));

            var returnCalendarService = new Mock<IReturnCalendarService>();

            var failedJobsQueryService = new Mock<IFailedJobQueryServiceILR>();

            var dateTimeProvider = new Mock<IDateTimeProvider>();

            var pathStructureService = new Mock<IPathStructureServiceILR>();

            var dasPaymentsService = new Mock<IDASPaymentsService>();

            var jobQueryService = new Mock<IJobQueryService>();

            var periodEndService = new PeriodEndServiceILR(loggerMock.Object, pathControllers, null, null, null, scheduleRepo.Object, dasEnd.Object, paramFactory.Object, validityPeriod.Object, returnCalendarService.Object, failedJobsQueryService.Object, dateTimeProvider.Object, pathStructureService.Object, dasPaymentsService.Object, jobQueryService.Object);

            await periodEndService.ProceedAsync(pathIds, year, period, CancellationToken.None);

            matchedPathController.Verify(c => c.Execute(year, period), Times.Once);
            subPathController.Verify(c => c.Execute(year, period), Times.Once);
        }

        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ProceedAsync_SubPathNotCalledIfNotValidForPeriod()
        {
            var pathId = 0;
            var pathIds = new List<int> { pathId };

            var subPathId = 1;
            var subPathIds = new List<int> { subPathId };

            var year = 1819;
            var period = 1;

            var loggerMock = new Mock<ILogger>();

            var matchedPathController = new Mock<ICriticalPathController>();
            matchedPathController.Setup(c => c.IsMatch(pathId)).Returns(true);
            matchedPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(true);
            matchedPathController.Setup(c => c.Execute(year, period)).ReturnsAsync(subPathIds);

            var subPathController = new Mock<ICriticalPathController>();
            subPathController.Setup(c => c.IsMatch(subPathId)).Returns(true);
            subPathController.Setup(m => m.IsValid(year, period)).ReturnsAsync(false);

            var pathControllers = new List<ICriticalPathController>
            {
                matchedPathController.Object,
                subPathController.Object
            };

            var scheduleRepo = new Mock<IScheduleRepository>();

            var dasEnd = new Mock<IDASStoppingPathItem>();

            var paramFactory = new Mock<IPathItemParamsFactory>();

            var validityPeriod = new Mock<IValidityPeriodService>();
            validityPeriod.Setup(c => c.GetValidPeriodsForCollections(year)).ReturnsAsync(BuildValidPeriods(year));

            var returnCalendarService = new Mock<IReturnCalendarService>();

            var failedJobsQueryService = new Mock<IFailedJobQueryServiceILR>();

            var dateTimeProvider = new Mock<IDateTimeProvider>();

            var pathStructureService = new Mock<IPathStructureServiceILR>();

            var dasPaymentsService = new Mock<IDASPaymentsService>();

            var jobQueryService = new Mock<IJobQueryService>();

            var periodEndService = new PeriodEndServiceILR(loggerMock.Object, pathControllers, null, null, null, scheduleRepo.Object, dasEnd.Object, paramFactory.Object, validityPeriod.Object, returnCalendarService.Object, failedJobsQueryService.Object, dateTimeProvider.Object, pathStructureService.Object, dasPaymentsService.Object, jobQueryService.Object);

            await periodEndService.ProceedAsync(pathIds, year, period, CancellationToken.None);

            matchedPathController.Verify(c => c.Execute(year, period), Times.Once);
            subPathController.Verify(c => c.Execute(year, period), Times.Never);
        }
    }
}