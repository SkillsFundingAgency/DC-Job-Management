using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PathItemTests
    {
        [Fact]
        [Trait("Category", "PeriodEnd")]
        public async Task ExecuteAsync_AddJobIsCalled_AndPathTaskReturnModelCorrect_ForBlockingItem()
        {
            var currentPeriod = 1;
            var jobId = 1;
            var jobStatusType = JobStatusType.Ready;
            var jobIds = new List<long> { jobId };
            var ordinal = 1;
            var collectionName = "PE-DAS-Start1920";
            var year = 1920;

            var pathItemParams = new PathItemParams
            {
                Period = currentPeriod,
                Ordinal = ordinal,
                CollectionName = collectionName,
                CollectionYear = year
            };

            var job = new FileUploadJob
            {
                CollectionName = collectionName,
                Status = jobStatusType,
                Priority = 1,
                CreatedBy = "Test"
            };

            var pathTaskReturn = new PathItemReturn
            {
                BlockingTask = true,
                JobIds = jobIds,
                SubPaths = null
            };

            var logger = new Mock<ILogger>();

            var jobManager = new Mock<IFileUploadJobManager>();
            jobManager
                .Setup(jm => jm.AddJob(job))
                .ReturnsAsync(jobId);

            var jobFactory = new Mock<IPeriodEndJobFactory>();
            jobFactory
                .Setup(jf => jf.CreateJobAsync(collectionName, year, currentPeriod, 0, string.Empty, string.Empty))
                .ReturnsAsync(job);

            var returnFactory = new Mock<IPathItemReturnFactory>();
            returnFactory
                .Setup(rf => rf.CreatePathTaskReturn(true, jobIds, null))
                .Returns(pathTaskReturn);

            var stateService = new Mock<IStateService>();

            var sut = new DASStarting(logger.Object, jobManager.Object, jobFactory.Object, returnFactory.Object, stateService.Object);

            var result = await sut.ExecuteAsync(pathItemParams);

            jobManager.Verify(jm => jm.AddJob(job), Times.Once);
            Assert.True(result.BlockingTask);
            Assert.True(result.SubPaths == null);
        }
    }
}