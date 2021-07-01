using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class ReturnCodeServiceTests
    {
        private readonly Mock<IESFReturnPeriodHelper> _mockEsfReturnPeriodHelper;
        private readonly Mock<IAppsReturnPeriodHelper> _mockAppsReturnPeriodHelper;
        private readonly ReturnCodeService _sut;

        public ReturnCodeServiceTests()
        {
            _mockEsfReturnPeriodHelper = new Mock<IESFReturnPeriodHelper>();
            _mockAppsReturnPeriodHelper = new Mock<IAppsReturnPeriodHelper>();

            _sut = new ReturnCodeService(_mockEsfReturnPeriodHelper.Object, _mockAppsReturnPeriodHelper.Object);
        }

        [Theory]

        [InlineData("ILR", 1920, 1, "ILR1920", "R01")]
        [InlineData("ILR", 1920, 10, "ILR1920", "R10")]
        [InlineData("ILR", 2021, 2, "ILR2021", "R02")]
        [InlineData("ILR", 2021, 12, "ILR2021", "R12")]

        [InlineData("NCS", 1920, 1, "NCS1920", "N01")]
        [InlineData("NCS", 1920, 10, "NCS1920", "N10")]
        [InlineData("NCS", 2021, 2, "NCS2021", "N02")]
        [InlineData("NCS", 2021, 12, "NCS2021", "N12")]

        [InlineData("ESF", 1920, 9, "ESF", "ESF52")]

        [InlineData("APPS", 1920, 9, "APPS", "APPS36")]

        public async Task IlrAppendYearAndPeriod(string type, int year, int period, string expectedCollection, string expectedPeriod)
        {
            // Arrange
            _mockEsfReturnPeriodHelper.Setup(x => x.GetESFReturnPeriod(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(period + 43);
            _mockAppsReturnPeriodHelper.Setup(x => x.GetReturnPeriod(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(period + 27);

            // Act
            var result = await _sut.GetReturnCodeForPeriodAsync(type, year, period);

            // Assert
            result.Should().NotBeNull();
            result.Collection.Should().Be(expectedCollection);
            result.Period.Should().Be(expectedPeriod);
        }

        [Fact]
        public async Task GetReturnCodeForPreviousPeriodAsync_WithUnsupportedCollection_RaisesError()
        {
            // Act
            Task result = _sut.GetReturnCodeForPreviousPeriodAsync("UnsupportedCollection", 2020, 2);

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await result);
        }

        [Fact]
        public async Task GetReturnCodeForPreviousPeriodAsync_ReturnsPreviousPeriod()
        {
            // Act
            var result = await _sut.GetReturnCodeForPreviousPeriodAsync(CollectionTypeConstants.Ilr, 2020, 2);

            // Assert
            Assert.Equal("R01", result.Period);
        }

        [Fact]
        public async Task GetReturnCodeForPeriodAsync_WithUnsupportedCollection_RaisesError()
        {
            // Act
            Task result = _sut.GetReturnCodeForPeriodAsync("UnsupportedCollection", 2020, 2);

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await result);
        }
    }
}
