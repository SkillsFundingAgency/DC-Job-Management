using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests.Strategies
{
    public class NonSubmittingProvidersTests
    {
        [Fact]
        public async Task ExecuteAsync_Returns_Valid_PathTaskReturn_Object()
        {
            var period = 1;
            var collectionYear = 2021;
            var ordinal = 0;

            var jobId = 1;
            var ukPrn = 12345678;
            var jobReadyStatus = 1;
            var ilr1929CollectionName = "ILR2021";
            var jobCollectionName = "PE-DAS-Submission2021";

            var pathItemParams = new PathItemParams
            {
                CollectionName = ilr1929CollectionName,
                Period = period,
                CollectionYear = collectionYear,
                Ordinal = ordinal
            };

            var providers = new List<ProviderJob>
            {
                new ProviderJob
                {
                    UkPrn = ukPrn
                }
            };

            var job = new FileUploadJob
            {
                CollectionName = jobCollectionName,
                Status = JobStatusType.Ready,
                CollectionYear = collectionYear,
                PeriodNumber = period,
                Ukprn = ukPrn
            };

            var pathItemJobs = new List<PathItemJobModel>
            {
                new PathItemJobModel
                {
                    JobId = jobId,
                    Status = jobReadyStatus
                }
            };

            var pathItem = new PathItemModel
            {
                Name = "foo",
                Ordinal = ordinal,
                PathId = 0,
                PathItemJobs = pathItemJobs,
                IsPausing = true
            };

            var jobIds = new List<long>
            {
                jobId
            };

            var pathItemReturn = new PathItemReturn
            {
                BlockingTask = true,
                JobIds = jobIds
            };

            var queryService = new Mock<IQueryService>();
            queryService
                .Setup(qs => qs.GetNonSubmittingProviders(pathItemParams.CollectionName, pathItemParams.Period))
                .ReturnsAsync(providers);

            var jobFactory = new Mock<IPeriodEndJobFactory>();
            jobFactory
                .Setup(jf => jf.CreateJobAsync(jobCollectionName, collectionYear, period, ukPrn, null, null))
                .ReturnsAsync(job);

            var jobManager = new Mock<IFileUploadJobManager>();
            jobManager
                .Setup(jm => jm.AddJob(job))
                .ReturnsAsync(jobId);

            var stateService = new Mock<IStateService>();
            stateService
                .Setup(ss => ss.SavePathItem(pathItem, collectionYear, period));

            var returnFactory = new Mock<IPathItemReturnFactory>();
            returnFactory
                .Setup(rf => rf.CreatePathTaskReturn(true, jobIds, null))
                .Returns(pathItemReturn);

            var nonSubmittingProvidersService = CreateService(queryService.Object, jobManager.Object, jobFactory.Object, returnFactory.Object, stateService.Object);

            var result = await nonSubmittingProvidersService.ExecuteAsync(pathItemParams);

            Assert.True(result.BlockingTask == pathItemReturn.BlockingTask);

            var index = 0;
            foreach (var id in result.JobIds)
            {
                Assert.True(id == pathItemReturn.JobIds.ToList()[index++]);
            }

            Assert.True(!result.SubPaths.Any());

            stateService.Verify(ss => ss.SavePathItem(It.IsAny<PathItemModel>(), collectionYear, period), Times.Once);
        }

        private NonSubmittingProviders CreateService(
            IQueryService queryService = null,
            IFileUploadJobManager fileUploadJobManager = null,
            IPeriodEndJobFactory periodEndJobFactory = null,
            IPathItemReturnFactory pathItemReturnFactory = null,
            IStateService stateService = null)
        {
            return new NonSubmittingProviders(queryService, fileUploadJobManager, Mock.Of<ILogger>(), periodEndJobFactory, pathItemReturnFactory, stateService);
        }
    }
}