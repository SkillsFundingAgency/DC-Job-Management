using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Services;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PeriodEndDateTimeServiceTests
    {
        public static IEnumerable<object[]> NullDate_TestData()
        {
            yield return new object[] { new DateTime(2019, 12, 31), null };
            yield return new object[] { null, new DateTime(2019, 01, 01) };
        }

        public static IEnumerable<object[]> SuccessfulSimple_TestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1, 13, 12, 16), new DateTime(2020, 2, 1, 15, 38, 34), 31, 2, 26, 18 };
            yield return new object[] { new DateTime(2020, 1, 1, 12, 0, 0), new DateTime(2020, 1, 2, 12, 0, 0), 1, 0, 0, 0 };
            yield return new object[] { new DateTime(2020, 4, 3, 23, 15, 0), new DateTime(2020, 4, 27, 0, 0, 0), 23, 0, 45, 0 };
        }

        public static IEnumerable<object[]> SuccessfulBusiness_Testdata()
        {
            yield return new object[] { new DateTime(2020, 2, 29, 2, 3, 12), new DateTime(2020, 3, 1, 14, 23, 54), 0, 0, 0, 0 };
            yield return new object[] { new DateTime(2020, 2, 26, 14, 27, 54), new DateTime(2020, 3, 01, 14, 23, 54), 2, 9, 32, 6 };
            yield return new object[] { new DateTime(2019, 5, 24, 13, 43, 16), new DateTime(2019, 6, 24, 14, 54, 35), 21, 1, 11, 19 };
        }

        [Theory]
        [MemberData(nameof(NullDate_TestData))]
        public void CalculateRunTimeSimpleWithNullData(DateTime? startDate, DateTime? endDate)
        {
            var periodEndDateTimeService = new PeriodEndDateTimeService();
            var result = periodEndDateTimeService.CalculateRuntimeSimple(startDate, endDate);
            result.Should().Be(default(TimeSpan));
        }

        [Theory]
        [MemberData(nameof(SuccessfulSimple_TestData))]
        public void CalculateRunTimeSimpleSuccess(DateTime? startDate, DateTime? endDate, int noOfDays, int noOfHours, int noOfMinutes, int noOfSeconds)
        {
            var periodEndDateTimeService = new PeriodEndDateTimeService();
            var result = periodEndDateTimeService.CalculateRuntimeSimple(startDate, endDate);
            result.Should().Be(new TimeSpan(noOfDays, noOfHours, noOfMinutes, noOfSeconds));
        }

        [Theory]
        [MemberData(nameof(NullDate_TestData))]
        public async Task CalculateRuntImeBusinessWithNullDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var periodEndDateTimeService = new PeriodEndDateTimeService();
            var baseresult = await periodEndDateTimeService.CalculateRuntimeBusiness(startDate, endDate);
            var result = baseresult.TimeDifference;
            result.Should().Be(default(TimeSpan));
        }

        [Theory]
        [MemberData(nameof(SuccessfulBusiness_Testdata))]
        public async Task CalculateRunTimeBusinessSuccessAsync(DateTime? startDate, DateTime? endDate, int noOfDays, int noOfHours, int noOfMinutes, int noOfSeconds)
        {
            var periodEndDateTimeService = new PeriodEndDateTimeService();
            var testDate = new DateTime(2020, 2, 3);
            var baseresult = await periodEndDateTimeService.CalculateRuntimeBusiness(startDate, endDate);
            var result = baseresult.TimeDifference;
            result.Should().Be(new TimeSpan(noOfDays, noOfHours, noOfMinutes, noOfSeconds));
        }
    }
}
