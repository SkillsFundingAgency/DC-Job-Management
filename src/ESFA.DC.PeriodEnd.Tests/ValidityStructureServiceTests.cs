using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services;
using ESFA.DC.PeriodEnd.Utils;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class ValidityStructureServiceTests
    {
        private readonly Mock<IPathStructureServiceILR> _mockPathStructureService;
        private readonly Mock<IValidityPeriodService> _mockValidityPeriodService;
        private readonly Mock<IValidityPeriodRepository> _mockValidityPeriodRepository;
        private readonly Mock<IPeriodEndRepository> _mockPeriodEndRepository;
        private readonly Mock<IDASStoppingPathItem> _mockDasStop;
        private readonly ValidityStructureService _sut;

        public ValidityStructureServiceTests()
        {
            _mockPathStructureService = new Mock<IPathStructureServiceILR>();
            _mockValidityPeriodService = new Mock<IValidityPeriodService>();
            _mockValidityPeriodRepository = new Mock<IValidityPeriodRepository>();
            _mockPeriodEndRepository = new Mock<IPeriodEndRepository>();
            _mockDasStop = new Mock<IDASStoppingPathItem>();

            _sut = new ValidityStructureService(
                                                _mockPathStructureService.Object,
                                                _mockValidityPeriodService.Object,
                                                _mockValidityPeriodRepository.Object,
                                                _mockPeriodEndRepository.Object,
                                                _mockDasStop.Object);
        }

        [Fact]
        public async Task GetAllPeriodEndItems_WithNullPeriod_RaisesArgumentException()
        {
            // Act
            Task result = _sut.GetAllPeriodEndItems(null, System.Threading.CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task GetAllPeriodEndItems_ReturnsDasStopOnCriticalPath()
        {
            // Arrange
            var dasStopDisplayName = "DASSTOP";
            var period = new CollectionsManagement.Models.YearPeriod();

            _mockDasStop.SetupGet(s => s.DisplayName).Returns(dasStopDisplayName);
            _mockValidityPeriodRepository.Setup(s => s.GetPathValidityPeriods(period.Year, period.Period, System.Threading.CancellationToken.None)).ReturnsAsync(new Dictionary<int, Models.SubPathValidityPeriodLookupModel>());
            _mockValidityPeriodRepository.Setup(s => s.GetValidityPeriodList(period.Year, period.Period, System.Threading.CancellationToken.None)).ReturnsAsync(new List<Models.ValidityPeriodLookupModel>());

            _mockPathStructureService.Setup(s => s.GetPathStructures(period, null, null, null, true, true, System.Threading.CancellationToken.None))
                .ReturnsAsync(new List<PathPathItemsModel>
                {
                    new PathPathItemsModel
                    {
                        PathId = (int)PeriodEndPath.ILRCriticalPath,
                        PathItems = new List<PathItemModel>()
                    }
                });

            // Act
            var result = await _sut.GetAllPeriodEndItems(period, System.Threading.CancellationToken.None);

            // Assert
            Assert.Single(result.Paths.Single(p => p.IsCritical).PathItems.Where(pi => pi.Name == dasStopDisplayName));
        }
    }
}
