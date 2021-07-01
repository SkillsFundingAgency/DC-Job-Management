using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class FisReferenceDataJobMetaDataServiceTests
    {
        [Fact]
        public async Task GetVersionNumberForJobId()
        {
            var cancellationToken = CancellationToken.None;
            var jobId = 100;

            var fisVersions = new List<FisJobMetaData>
            {
                new FisJobMetaData { JobId = 100, VersionNumber = 1 },
                new FisJobMetaData { JobId = 200, VersionNumber = 2 },
                new FisJobMetaData { JobId = 300, VersionNumber = 3 },
            }.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(x => x.FisJobMetaData).Returns(fisVersions.Object);

            var contextFactoryMock = new Mock<Func<IJobQueueDataContext>>();
            contextFactoryMock.Setup(x => x.Invoke()).Returns(contextMock.Object);

            var version = await NewService(contextFactoryMock.Object).GetVersionNumberForJobId(jobId, cancellationToken);

            version.Should().Be(1);
        }

        [Fact]
        public async Task GetFisJobMetaDataForJobId()
        {
            var cancellationToken = CancellationToken.None;
            var jobId = 100;

            var fisVersions = new List<FisJobMetaData>
            {
                new FisJobMetaData { JobId = 100, VersionNumber = 1 },
                new FisJobMetaData { JobId = 200, VersionNumber = 2 },
                new FisJobMetaData { JobId = 300, VersionNumber = 3 },
            }.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(x => x.FisJobMetaData).Returns(fisVersions.Object);

            var contextFactoryMock = new Mock<Func<IJobQueueDataContext>>();
            contextFactoryMock.Setup(x => x.Invoke()).Returns(contextMock.Object);

            var expectedModel = new FisJobMetaData { JobId = 100, VersionNumber = 1 };

            var model = await NewService(contextFactoryMock.Object).GetFisJobMetaDataForJobId(jobId, cancellationToken);

            model.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task SetGeneratedDateForJobId()
        {
            var cancellationToken = CancellationToken.None;
            var jobId = 100;
            var generatedDate = new DateTime(2020, 8, 1);

            var fisVersions = new List<FisJobMetaData>
            {
                new FisJobMetaData { JobId = 100, VersionNumber = 1 },
                new FisJobMetaData { JobId = 200, VersionNumber = 2 },
                new FisJobMetaData { JobId = 300, VersionNumber = 3 },
            }.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(x => x.FisJobMetaData).Returns(fisVersions.Object);

            var contextFactoryMock = new Mock<Func<IJobQueueDataContext>>();
            contextFactoryMock.Setup(x => x.Invoke()).Returns(contextMock.Object);

            await NewService(contextFactoryMock.Object).SetGeneratedDateForJobId(jobId, generatedDate, cancellationToken);

            contextMock.Object.FisJobMetaData.SingleOrDefault(x => x.JobId == jobId).GeneratedDate.Should().Be(generatedDate);
        }

        [Fact]
        public async Task SetPublishedDateForJobId()
        {
            var cancellationToken = CancellationToken.None;
            var jobId = 100;
            var publishedDate = new DateTime(2020, 8, 1);

            var fisVersions = new List<FisJobMetaData>
            {
                new FisJobMetaData { JobId = 100, VersionNumber = 1 },
                new FisJobMetaData { JobId = 200, VersionNumber = 2 },
                new FisJobMetaData { JobId = 300, VersionNumber = 3 },
            }.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(x => x.FisJobMetaData).Returns(fisVersions.Object);

            var contextFactoryMock = new Mock<Func<IJobQueueDataContext>>();
            contextFactoryMock.Setup(x => x.Invoke()).Returns(contextMock.Object);

            await NewService(contextFactoryMock.Object).SetPublishedDateForJobId(jobId, publishedDate, cancellationToken);

            contextMock.Object.FisJobMetaData.SingleOrDefault(x => x.JobId == jobId).PublishedDate.Should().Be(publishedDate);
        }

        [Fact]
        public async Task SetRemovedFlagForJobId()
        {
            var cancellationToken = CancellationToken.None;
            var jobId = 100;

            var fisVersions = new List<FisJobMetaData>
            {
                new FisJobMetaData { JobId = 100, VersionNumber = 1 },
                new FisJobMetaData { JobId = 200, VersionNumber = 2 },
                new FisJobMetaData { JobId = 300, VersionNumber = 3 },
            }.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<IJobQueueDataContext>();
            contextMock.Setup(x => x.FisJobMetaData).Returns(fisVersions.Object);

            var contextFactoryMock = new Mock<Func<IJobQueueDataContext>>();
            contextFactoryMock.Setup(x => x.Invoke()).Returns(contextMock.Object);

            await NewService(contextFactoryMock.Object).SetRemovedFlagForJobId(jobId, cancellationToken);

            contextMock.Object.FisJobMetaData.SingleOrDefault(x => x.JobId == jobId).IsRemoved.Should().Be(true);
        }

        private FisReferenceDataJobMetaDataService NewService(Func<IJobQueueDataContext> contextFactory)
        {
            return new FisReferenceDataJobMetaDataService(contextFactory, Mock.Of<ILogger>());
        }
    }
}
