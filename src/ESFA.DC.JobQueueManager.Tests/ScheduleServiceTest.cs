using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.ExternalData;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public sealed class ScheduleServiceTest
    {
        [Fact]
        public async Task CanExecuteScheduleLastExecutionTime()
        {
            var dateTimeUtcNow = new DateTime(DateTime.UtcNow.Year, 10, 21, 16, 0, 0, DateTimeKind.Utc);
            var schedule = new Schedule
            {
                Month = 10,
                DayOfTheMonth = 21,
                Hour = 16,
                Minute = 0,
                LastExecuteDateTime = dateTimeUtcNow
            };

            var scheduleService = GetService();
            var canExecuteSchedule = await scheduleService.CanExecuteSchedule(schedule, dateTimeUtcNow, true);

            canExecuteSchedule.Should().BeFalse();
        }

        [Fact]
        public async Task CanExecuteScheduleRemoveOldDateFalse()
        {
            var dateTimeUtcNow = new DateTime(DateTime.UtcNow.Year, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 10,
                DayOfTheMonth = 21,
                Hour = 16,
                Minute = 0
            };

            var scheduleService = GetService();
            var canExecuteSchedule = await scheduleService.CanExecuteSchedule(schedule, dateTimeUtcNow, false);

            canExecuteSchedule.Should().BeTrue();
        }

        [Fact]
        public async Task CanExecuteScheduleRemoveOldDateTrue()
        {
            var dateTimeUtcNow = new DateTime(DateTime.UtcNow.Year, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 10,
                DayOfTheMonth = 21,
                Hour = 16,
                Minute = 0
            };

            var scheduleService = GetService();
            var canExecuteSchedule = await scheduleService.CanExecuteSchedule(schedule, dateTimeUtcNow, true);

            canExecuteSchedule.Should().BeTrue();
        }

        [Fact]
        public void TestDayOfMonth()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);
            var schedule = new Schedule
            {
                DayOfTheMonth = 21
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(21);
        }

        [Fact]
        public void TestDayOfMonthAndDayOfWeekA()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc); // Wednesday

            var schedule = new Schedule
            {
                DayOfTheMonth = 23,
                DayOfTheWeek = (byte)DayOfWeek.Thursday
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(22);
        }

        [Fact]
        public void TestDayOfMonthAndDayOfWeekB()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc); // Wednesday

            var schedule = new Schedule
            {
                DayOfTheMonth = 23,
                DayOfTheWeek = (byte)DayOfWeek.Saturday
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(23);
        }

        [Fact]
        public void TestDayOfMonthNot()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                DayOfTheMonth = 1
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(1);
        }

        [Fact]
        public void TestDayOfMonthNotRemove()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                DayOfTheMonth = 1
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, true);

            nextRun.Should().BeNull();
        }

        [Fact]
        public void TestDayOfWeek()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc); // Wednesday

            var schedule = new Schedule
            {
                DayOfTheWeek = (byte)DayOfWeek.Thursday
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(22);
        }

        [Fact]
        public void TestDayOfWeekNot()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 31, 16, 0, 0, DateTimeKind.Utc); // Saturday

            var schedule = new Schedule
            {
                DayOfTheWeek = (byte)DayOfWeek.Thursday
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Day.Should().Be(5);
            nextRun?.Month.Should().Be(11);
        }

        [Fact]
        public void TestDayOfWeekNotRemove()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 31, 16, 0, 0, DateTimeKind.Utc); // Saturday

            var schedule = new Schedule
            {
                DayOfTheWeek = (byte)DayOfWeek.Thursday
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, true);

            nextRun.Should().BeNull();
        }

        [Fact]
        public void TestDefault()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var schedule = new Schedule();

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Month.Should().Be(dateTimeUtcNow.Month);
            nextRun?.Day.Should().Be(dateTimeUtcNow.Day);
            nextRun?.Hour.Should().Be(dateTimeUtcNow.Hour);
            nextRun?.Minute.Should().Be(dateTimeUtcNow.Minute);
        }

        [Fact]
        public void TestHour()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Hour = 15
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Hour.Should().Be(15);
        }

        [Fact]
        public void TestHourNot()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Hour = 17
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Hour.Should().Be(17);
        }

        [Fact]
        public void TestHourNotRemove()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Hour = 15
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, true);

            nextRun.Should().BeNull();
        }

        [Fact]
        public void TestMinute()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 15, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Minute = 14
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Minute.Should().Be(14);
        }

        [Fact]
        public void TestMinuteCadence()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 15, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                MinuteIsCadence = true,
                Minute = 5
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Minute.Should().Be(15);
        }

        [Fact]
        public void TestMinuteCadenceEnd()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 57, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                MinuteIsCadence = true,
                Minute = 5
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Minute.Should().Be(0);
        }

        [Fact]
        public void TestMinuteCadenceStart()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                MinuteIsCadence = true,
                Minute = 5
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Minute.Should().Be(0);
        }

        [Fact]
        public void TestMinuteNot()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 15, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Minute = 16
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Minute.Should().Be(16);
        }

        [Fact]
        public void TestMinuteNotRemove()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 15, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Minute = 14
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, true);

            nextRun.Should().BeNull();
        }

        [Fact]
        public void TestMonth()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 10
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Month.Should().Be(10);
        }

        [Fact]
        public void TestMonthNot()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 9
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, false);

            nextRun.Should().NotBeNull();
            nextRun?.Month.Should().Be(9);
        }

        [Fact]
        public void TestMonthNotRemove()
        {
            var dateTimeUtcNow = new DateTime(2015, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 9
            };

            var scheduleService = GetService();
            var nextRun = scheduleService.GetNextRun(dateTimeUtcNow, schedule, true);

            nextRun.Should().BeNull();
        }

        [Fact]
        public async Task TestJobsInProgress_CanExecute_False()
        {
            var dateTimeUtcNow = new DateTime(DateTime.UtcNow.Year, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 10,
                DayOfTheMonth = 21,
                Hour = 16,
                Minute = 0,
                CollectionId = 21,
                Collection = new Collection
                {
                    CollectionId = 1,
                    Name = "Ref",
                },
            };

            var jobQueryServiceMock = new Mock<IJobQueryService>();
            jobQueryServiceMock.Setup(x => x.IsAnyJobInProgressAsync(21, null, CancellationToken.None)).ReturnsAsync(true);

            var scheduleService = GetService(jobQueryServiceMock.Object);
            var canRun = await scheduleService.CanExecuteSchedule(schedule, dateTimeUtcNow, true);

            canRun.Should().BeFalse();
        }

        [Fact]
        public async Task TestJobsInProgress_CanExecute_True()
        {
            var dateTimeUtcNow = new DateTime(DateTime.UtcNow.Year, 10, 21, 16, 0, 0, DateTimeKind.Utc);

            var schedule = new Schedule
            {
                Month = 10,
                DayOfTheMonth = 21,
                Hour = 16,
                Minute = 0,
                CollectionId = 21,
                Collection = new Collection
                {
                    CollectionId = 1,
                    Name = "Ref",
                }
            };

            var scheduleService = GetService();
            var canRun = await scheduleService.CanExecuteSchedule(schedule, dateTimeUtcNow, true);

            canRun.Should().BeTrue();
        }

        private ScheduleService GetService(IJobQueryService jobQueryService = null)
        {
            var jobQueryServiceMock = new Mock<IJobQueryService>();
            jobQueryServiceMock.Setup(x => x.IsAnyJobInProgressAsync(21, null, CancellationToken.None)).ReturnsAsync(false);

            var scheduleService = new ScheduleService(new Mock<ILogger>().Object, jobQueryService ?? jobQueryServiceMock.Object);

            return scheduleService;
        }
    }
}