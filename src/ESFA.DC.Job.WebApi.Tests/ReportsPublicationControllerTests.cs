using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Controllers;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ESFA.DC.Job.WebApi.Tests
{
    public class ReportsPublicationControllerTests
    {
        [Fact]
        public async Task UnpublishFrmReportsControllerTest()
        {
            // Arrange
            var collectionName = "frm-1920";
            var periodNumber = 7;

            var outputServiceMock = new Mock<IReportsPublicationJobMetaDataService>();
            var controller = new ReportsPublicationController(new Mock<ILogger>().Object, null, null, outputServiceMock.Object, null);
            // Act
            var actionResult = await controller.UnpublishFrmReportsAsync(collectionName, periodNumber, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
            outputServiceMock.Verify(r => r.MarkAsUnPublishedAsync(collectionName, periodNumber, CancellationToken.None));
        }
    }
}
