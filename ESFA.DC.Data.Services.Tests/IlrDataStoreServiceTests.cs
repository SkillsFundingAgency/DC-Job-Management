using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Data.Services.Tests
{
    public class IlrDataStoreServiceTests
    {
        [Fact]
        public async void Ilr1920Test()
        {
            var fileDetails = new IlrFileDetails();

            long ukprn = 999999;
            int academicYear1920 = 1920;
            int academicYear2001 = 2001;

            IEnumerable<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsServices = new List<IIlrGetSubmissionDetailsService>()
            {
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
                BuildServiceMock(academicYear2001, ukprn, null),
            };

            var ilrDataStoreService = NewService(services: ilrSubmissionDetailsServices);

            var result = await ilrDataStoreService.GetLatestIlrSubmissionDetails(academicYear1920, ukprn);

            result.Should().BeSameAs(fileDetails);
        }

        // no year match

        [Fact]
        public void IlrNoYearTest()
        {
            var fileDetails = new IlrFileDetails();
            long ukprn = 999999;
            int academicYear1920 = 1920;
            IEnumerable<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsServices = new List<IIlrGetSubmissionDetailsService>()
            {
                null,
            };
            var ilrDataStoreService = NewService(services: ilrSubmissionDetailsServices);

            Func<Task> action = async () => await ilrDataStoreService.GetLatestIlrSubmissionDetails(academicYear1920, ukprn);

            // sequence contains no elements so should throw nullreferenceexception
            action.Should().Throw<NullReferenceException>();
        }

        // single year registered

        [Fact]
        public async void SingleYearTest()
        {
            var fileDetails = new IlrFileDetails();

            long ukprn = 999999;
            int academicYear1920 = 1920;

            IEnumerable<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsServices = new List<IIlrGetSubmissionDetailsService>()
            {
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
            };

            var ilrDataStoreService = NewService(services: ilrSubmissionDetailsServices);

            var result = await ilrDataStoreService.GetLatestIlrSubmissionDetails(academicYear1920, ukprn);

            result.Should().BeSameAs(fileDetails);
        }

        // more than one year registered crash

        [Fact]
        public void MoreThanOneYearRegistered()
        {
            var fileDetails = new IlrFileDetails();
            long ukprn = 999999;
            int academicYear1920 = 1920;

            IEnumerable<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsServices = new List<IIlrGetSubmissionDetailsService>()
            {
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
                BuildServiceMock(academicYear1920, ukprn, fileDetails),
            };

            var ilrDataStoreService = NewService(services: ilrSubmissionDetailsServices);

            Func<Task> action = async () => await ilrDataStoreService.GetLatestIlrSubmissionDetails(academicYear1920, ukprn);

            // sequence contains identical elements so should throw invalidoperationException
            action.Should().Throw<InvalidOperationException>();
        }

        private IlrDataStoreService NewService(ILogger logger = null, IEnumerable<IIlrGetSubmissionDetailsService> services = null)
        {
            return new IlrDataStoreService(
                logger ?? Mock.Of<ILogger>(),
                services ?? Array.Empty<IIlrGetSubmissionDetailsService>());
        }

        private IIlrGetSubmissionDetailsService BuildServiceMock(int academicYear, long ukprn, IlrFileDetails ilrFileDetails)
        {
            Mock<IIlrGetSubmissionDetailsService> ilrSubmissionDetailsService = new Mock<IIlrGetSubmissionDetailsService>();

            ilrSubmissionDetailsService.SetupGet(x => x.AcademicYear).Returns(academicYear);
            ilrSubmissionDetailsService.Setup(x => x.GetIlrSubmissionDetails(ukprn)).ReturnsAsync(ilrFileDetails);

            return ilrSubmissionDetailsService.Object;
        }
    }
}
