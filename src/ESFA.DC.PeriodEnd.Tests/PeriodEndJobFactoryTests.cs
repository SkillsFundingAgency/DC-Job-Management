using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.PeriodEnd.Services.Factories;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PeriodEndJobFactoryTests
    {
        private readonly Mock<ICollectionService> _mockCollectionService;
        private readonly PeriodEndJobFactory _sut;

        public PeriodEndJobFactoryTests()
        {
            _mockCollectionService = new Mock<ICollectionService>();
            _sut = new PeriodEndJobFactory(_mockCollectionService.Object);
        }

        [Fact]
        public async Task CreateJobAsync_WithNoStorageReference_GetsFromCollectionService()
        {
            // Act
            var result = await _sut.CreateJobAsync(null, 0, 0);

            // Assert
            _mockCollectionService.Verify(v => v.GetCollectionFromNameAsync(It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Once());
        }
    }
}
