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
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests.Strategies
{
    public class DASReprocessingTests
    {
        [Fact]
        public async Task ExecuteAsync_DAS_Processed_All()
        {
            var jobId = 1;
            var jobReadyStatus = 1;
            var ukPrn = 12345678;
            var period = 1;
            var collectionYear = 2021;
            var ordinal = 0;
            var ilrCollectionName = "ILR2021";
            var dasSubmissionCollectionName = "PE-DAS-Submission2021";

            var pathItemParams = new PathItemParams
            {
                CollectionName = dasSubmissionCollectionName,
                Period = period,
                CollectionYear = collectionYear,
                Ordinal = ordinal
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

            var providers = new List<ProviderJob>
            {
                new ProviderJob
                {
                    UkPrn = ukPrn,
                    JobId = jobId
                }
            };

            var dasProcessedJobs = new List<DASSubmission>
            {
                new DASSubmission
                {
                    JobId = jobId,
                    UkPrn = ukPrn
                }
            };

            var pathItemReturn = new PathItemReturn
            {
                BlockingTask = true,
                JobIds = Enumerable.Empty<long>()
            };

            var pathItemReturnFactoryMock = new Mock<IPathItemReturnFactory>();
            pathItemReturnFactoryMock
                .Setup(m => m.CreatePathTaskReturn(true, null, null))
                .Returns(pathItemReturn);

            var stateServiceMock = new Mock<IStateService>();
            stateServiceMock
                .Setup(m => m.SavePathItem(pathItem, collectionYear, period));

            var queryServiceMock = new Mock<IQueryService>();
            queryServiceMock
                .Setup(qs => qs.GetLatestDASSubmittedJobs(ilrCollectionName, pathItemParams.CollectionName))
                .ReturnsAsync(providers);

            var dasClientServiceMock = new Mock<IDASClientService>();
            dasClientServiceMock
                .Setup(m => m.GetDASProcessedProviders(collectionYear, period))
                .ReturnsAsync(dasProcessedJobs);

            var service = CreateService(
                Mock.Of<IFileUploadJobManager>(),
                Mock.Of<IPeriodEndJobFactory>(),
                pathItemReturnFactoryMock.Object,
                stateServiceMock.Object,
                queryServiceMock.Object,
                dasClientServiceMock.Object);

            var result = await service.ExecuteAsync(pathItemParams);

            result.JobIds.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_DAS_Missed_Some_Providers()
        {
            var jobId = 1;
            var jobReadyStatus = 1;
            var ukPrn = 12345678;
            var period = 1;
            var collectionYear = 2021;
            var ordinal = 0;
            var ilrCollectionName = "ILR2021";
            var dasSubmissionCollectionName = "PE-DAS-Submission2021";

            var pathItemParams = new PathItemParams
            {
                CollectionName = dasSubmissionCollectionName,
                Period = period,
                CollectionYear = collectionYear,
                Ordinal = ordinal
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

            var job = new FileUploadJob
            {
                CollectionName = dasSubmissionCollectionName,
                Status = JobStatusType.Ready,
                CollectionYear = collectionYear,
                PeriodNumber = period,
                Ukprn = ukPrn
            };

            var providers = new List<ProviderJob>
            {
                new ProviderJob
                {
                    UkPrn = ukPrn,
                    JobId = jobId
                }
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

            var fileUploadJobManagerMock = new Mock<IFileUploadJobManager>();
            fileUploadJobManagerMock
                .Setup(m => m.AddJob(job))
                .ReturnsAsync(jobId);

            var periodEndJobFactoryMock = new Mock<IPeriodEndJobFactory>();
            periodEndJobFactoryMock
                .Setup(m => m.CreateJobAsync(dasSubmissionCollectionName, collectionYear, period, ukPrn, string.Empty, string.Empty))
                .ReturnsAsync(job);

            var pathItemReturnFactoryMock = new Mock<IPathItemReturnFactory>();
            pathItemReturnFactoryMock
                .Setup(m => m.CreatePathTaskReturn(true, JobIdsMatch(jobIds), null))
                .Returns(pathItemReturn);

            var stateServiceMock = new Mock<IStateService>();
            stateServiceMock
                .Setup(m => m.SavePathItem(pathItem, collectionYear, period));

            var queryServiceMock = new Mock<IQueryService>();
            queryServiceMock
                .Setup(qs => qs.GetLatestDASSubmittedJobs(ilrCollectionName, dasSubmissionCollectionName))
                .ReturnsAsync(providers);

            var dasClientServiceMock = new Mock<IDASClientService>();
            dasClientServiceMock
                .Setup(m => m.GetDASProcessedProviders(collectionYear, period))
                .ReturnsAsync(Enumerable.Empty<DASSubmission>());

            var service = CreateService(
                fileUploadJobManagerMock.Object,
                periodEndJobFactoryMock.Object,
                pathItemReturnFactoryMock.Object,
                stateServiceMock.Object,
                queryServiceMock.Object,
                dasClientServiceMock.Object);

            var result = await service.ExecuteAsync(pathItemParams);

            result.JobIds.Should().NotBeEmpty();
            result.JobIds.First().Should().Be(jobId);
        }

        public IEnumerable<long> JobIdsMatch(List<long> jobIds)
        {
            return Match.Create<List<long>>(m => m.All(jobIds.Contains));
        }

        private DASReprocessing CreateService(
            IFileUploadJobManager fileUploadJobManager = null,
            IPeriodEndJobFactory periodEndJobFactory = null,
            IPathItemReturnFactory pathItemReturnFactory = null,
            IStateService stateService = null,
            IQueryService queryService = null,
            IDASClientService dasClientService = null)
        {
            return new DASReprocessing(
                Mock.Of<ILogger>(),
                fileUploadJobManager,
                periodEndJobFactory,
                Mock.Of<IJsonSerializationService>(),
                pathItemReturnFactory,
                stateService,
                queryService,
                dasClientService);
        }
    }
}