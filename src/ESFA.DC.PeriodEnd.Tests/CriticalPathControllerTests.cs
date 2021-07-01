using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services;
using ESFA.DC.PeriodEnd.Services.ILR.Controllers;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class CriticalPathControllerTests : BaseTest
    {
        private readonly Mock<IPathItemParamsFactory> _pathItemParamsFactory;
        private readonly Mock<IStateService> _stateServiceMock;
        private readonly Mock<ILogger> _logger;
        private readonly Mock<IValidityPeriodService> _validityPeriod;
        private readonly Mock<IPeriodEndRepository> _periodEndRepository;

        public CriticalPathControllerTests()
        {
            _stateServiceMock = new Mock<IStateService>();
            _pathItemParamsFactory = new Mock<IPathItemParamsFactory>();
            _logger = new Mock<ILogger>();
            _validityPeriod = new Mock<IValidityPeriodService>();
            _periodEndRepository = new Mock<IPeriodEndRepository>();
        }

        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ExecuteAsync_CallsFirstItemOnlyIfBreakingTask()
        {
            // Arrange
            var period = 1;
            var listId = 0;
            var year = 1819;
            var ordinal = 0;
            var collectionName = "ILR1819";
            var pathId = 0;

            var pathItemParams = new PathItemParams
            {
                Period = period,
                Ordinal = ordinal + 1,
                CollectionName = collectionName
            };

            _pathItemParamsFactory
                .Setup(pip => pip.GetPathItemParams(ordinal, year, period, collectionName, pathId))
                .Returns(pathItemParams);
            _pathItemParamsFactory
                .Setup(pip => pip.GetPathItemParams(ordinal + 1, year, period, collectionName, pathId))
                .Returns(pathItemParams);

            _stateServiceMock
                .Setup(ss => ss.GetStateForPathId(listId, year, period))
                .ReturnsAsync(new PeriodEndJobState
                {
                    PathId = listId,
                    Position = 0,
                    Period = period,
                    Year = year,
                    CollectionName = collectionName,
                    IsBusy = false
                });

            _stateServiceMock
               .Setup(ss => ss.SavePathIsBusyAsync(pathId, true))
               .ReturnsAsync(true);

            var pathItem1 = CreateMockPathItem(period, year, true, true, null, pathItemParams);
            var pathItem2 = CreateMockPathItem(period, year, true, true, null, pathItemParams);

            var pathItems = new List<IILRPathItem>
            {
                pathItem1.Object,
                pathItem2.Object
            }.OrderBy(x => x, new PathComparer());

            _validityPeriod.Setup(c => c.GetValidPeriodsForCollections(year)).ReturnsAsync(BuildValidPeriods(year));

            var sut = new CriticalPathController(_stateServiceMock.Object, pathItems, _pathItemParamsFactory.Object, _validityPeriod.Object, _periodEndRepository.Object, _logger.Object);

            // Act
            await sut.Execute(year, period);

            // Assert
            pathItem1.Verify(p => p.ExecuteAsync(pathItemParams), Times.Once);
            pathItem2.Verify(p => p.ExecuteAsync(pathItemParams), Times.Never);
        }

        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ExecuteAsync_CallsBothItemsIfNonBreakingTask()
        {
            // Arrange
            var period = 1;
            var listId = 0;
            var year = 1819;
            var ordinal = 0;
            var collectionName = "ILR1819";
            var pathId = 0;

            var pathItemParams = new PathItemParams
            {
                Period = period,
                Ordinal = ordinal + 1,
                CollectionName = collectionName
            };

            _pathItemParamsFactory
                .Setup(pip => pip.GetPathItemParams(ordinal, year, period, collectionName, pathId))
                .Returns(pathItemParams);

            _pathItemParamsFactory
                .Setup(pip => pip.GetPathItemParams(ordinal + 1, year, period, collectionName, pathId))
                .Returns(pathItemParams);

            _stateServiceMock
                .Setup(ss => ss.GetStateForPathId(listId, year, period))
                .ReturnsAsync(new PeriodEndJobState
                {
                    PathId = listId,
                    Position = 0,
                    Period = period,
                    Year = year,
                    CollectionName = collectionName,
                    IsBusy = false
                });

            _stateServiceMock
               .Setup(ss => ss.SavePathIsBusyAsync(pathId, true))
               .ReturnsAsync(true);

            var pathItem1 = CreateMockPathItem(period, year, true, false, null, pathItemParams);
            var pathItem2 = CreateMockPathItem(period, year, true, true, new List<long> { 1, 2, 3 }, pathItemParams);

            var pathItems = new List<IILRPathItem>
            {
                pathItem1.Object,
                pathItem2.Object
            }.OrderBy(x => x, new PathComparer());

            _validityPeriod.Setup(c => c.GetValidPeriodsForCollections(year)).ReturnsAsync(BuildValidPeriods(year));

            var sut = new CriticalPathController(_stateServiceMock.Object, pathItems, _pathItemParamsFactory.Object, _validityPeriod.Object, _periodEndRepository.Object, _logger.Object);

            // Act
            await sut.Execute(year, period);

            // Assert
            pathItem1.Verify(p => p.ExecuteAsync(pathItemParams), Times.Once);
            pathItem2.Verify(p => p.ExecuteAsync(pathItemParams), Times.Once);
        }

        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ExecuteAsync_WhenPathIsBusy_NoneCalled()
        {
            var pathItemParams = new PathItemParams
            {
                Period = 1819,
                Ordinal = 2,
                CollectionName = "ILR1819"
            };

            var pathItem1 = CreateMockPathItem(2, 1819, true, false, null, pathItemParams);
            var pathItem2 = CreateMockPathItem(2, 1819, true, true, new List<long> { 1, 2, 3 }, pathItemParams);

            var pathItems = new List<IILRPathItem>
            {
                pathItem1.Object,
                pathItem2.Object
            }.OrderBy(x => x, new PathComparer());

            var sut = new CriticalPathController(_stateServiceMock.Object, pathItems, _pathItemParamsFactory.Object, _validityPeriod.Object, _periodEndRepository.Object, _logger.Object);

            // Act
            await sut.Execute(1, 1);

            // Assert
            pathItem1.Verify(p => p.ExecuteAsync(pathItemParams), Times.Never);
        }

        private Mock<IILRPathItem> CreateMockPathItem(int period, int year, bool isValid, bool isBlocking, IEnumerable<long> jobs, PathItemParams pathItemParams)
        {
            var pathItem = new Mock<IILRPathItem>();

            pathItem.Setup(pi => pi.IsValidForPeriod(period, year, It.IsAny<IDictionary<string, IEnumerable<int>>>()))
                .ReturnsAsync(isValid);

            pathItem
                .Setup(pi => pi.ExecuteAsync(pathItemParams))
                .ReturnsAsync(new PathItemReturn
                {
                    BlockingTask = isBlocking,
                    SubPaths = null,
                    JobIds = jobs
                });

            return pathItem;
        }
    }
}