using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Moq;
using Xunit;
using Collection = ESFA.DC.CollectionsManagement.Models.Collection;
using Job = ESFA.DC.Jobs.Model.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class JobConverterTests
    {
        [Fact]
        public async Task JobToJobEntity_Test_Null()
        {
            Job job = null;
            var jobConverter = GetJobConverter();
            var convertedJob = await jobConverter.Convert(job);
            convertedJob.Should().BeNull();
        }

        [Fact]
        public async Task JobEntityToJob_Test_Null()
        {
            Data.Entities.Job job = null;

            var jobConverter = GetJobConverter();
            var convertedJob = await jobConverter.Convert(job);
            convertedJob.Should().BeNull();
        }

        [Fact]
        public async Task JobToJobEntity_Test()
        {
            var currentTime = DateTime.UtcNow;
            var job = new Job
            {
                DateTimeCreatedUtc = currentTime,
                DateTimeUpdatedUtc = currentTime,
                JobId = 1,
                Priority = 1,
                RowVersion = "test",
                Status = JobStatusType.Ready,
                CreatedBy = "test",
                NotifyEmail = "test@test.com",
                CollectionName = CollectionConstants.ILR2021,
                CrossLoadingStatus = JobStatusType.Ready,
                Ukprn = 900,
            };

            var jobConverter = GetJobConverter();

            var convertedJob = await jobConverter.Convert(job);

            convertedJob.JobId.Should().Be(1);
            convertedJob.DateTimeCreatedUtc.Should().Be(currentTime);
            convertedJob.DateTimeUpdatedUtc.Should().Be(currentTime);
            convertedJob.CollectionId.Should().Be(1);
            convertedJob.Priority.Should().Be(1);
            convertedJob.Status.Should().Be(1);
            convertedJob.NotifyEmail.Should().Be("test@test.com");
            convertedJob.CreatedBy.Should().Be("test");
            convertedJob.CrossLoadingStatus.Should().Be((short)JobStatusType.Ready);
            convertedJob.Ukprn.Should().Be(900);
        }

        [Fact]
        public async Task JobEntityToJob_Test()
        {
            var currentTime = DateTime.UtcNow;
            var job = new Data.Entities.Job
            {
                DateTimeCreatedUtc = currentTime,
                DateTimeUpdatedUtc = currentTime,
                JobId = 1,
                CollectionId = 1,
                Priority = 1,
                RowVersion = null,
                Status = 1,
                NotifyEmail = "email@email.com",
                CreatedBy = "test",
                CrossLoadingStatus = (short)JobStatusType.Ready,
                Collection = new Data.Entities.Collection()
                {
                    CollectionId = 1,
                    Name = CollectionConstants.ILR2021,
                },
            };

            var convertedJob = await GetJobConverter().Convert(job);

            convertedJob.JobId.Should().Be(1);
            convertedJob.DateTimeCreatedUtc.Should().Be(currentTime);
            convertedJob.DateTimeUpdatedUtc.Should().Be(currentTime);
            convertedJob.CollectionName.Should().Be(CollectionConstants.ILR2021);
            convertedJob.Priority.Should().Be(1);
            convertedJob.Status.Should().Be(1);
            convertedJob.NotifyEmail.Should().Be("email@email.com");
            convertedJob.CreatedBy.Should().Be("test");
            convertedJob.CrossLoadingStatus.Should().Be(JobStatusType.Ready);
        }

        [Fact]
        public async Task JobMetaDataEntityToJobMetaData_Test()
        {
            var entity = new FileUploadJobMetaData
            {
                FileName = "test.xml",
                JobId = 1,
                StorageReference = "test-ref",
                PeriodNumber = 10,
                FileSize = 1000,
                Job = new Data.Entities.Job
                {
                    JobId = 1,
                    CollectionId = 1,
                    Ukprn = 1000,
                    Collection = new Data.Entities.Collection()
                    {
                        CollectionId = 1,
                        Name = CollectionConstants.ILR2021,
                    }
                }
            };

            var job = new FileUploadJob();
            await GetJobConverter().Convert(entity, job);

            job.JobId.Should().Be(1);
            job.FileName.Should().Be("test.xml");
            job.StorageReference.Should().Be("test-ref");
            job.Ukprn.Should().Be(1000);
            job.CollectionName.Should().Be(CollectionConstants.ILR2021);
            job.PeriodNumber.Should().Be(10);
            job.FileSize.Should().Be(1000);
        }

        private IJobConverter GetJobConverter()
        {
            var collectionSeviceMock = new Mock<ICollectionService>();
            collectionSeviceMock.Setup(x => x.GetCollectionFromNameAsync(CancellationToken.None, It.IsAny<string>())).ReturnsAsync(new Collection()
            {
                CollectionId = 1,
                CollectionTitle = CollectionConstants.ILR2021,
            });

            return new JobConverter(collectionSeviceMock.Object);
        }
    }
}
