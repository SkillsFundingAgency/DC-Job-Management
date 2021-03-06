// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using ESFA.DC.JobContext;
// using ESFA.DC.JobContext.Interface;
// using ESFA.DC.JobQueueManager.Interfaces;
// using ESFA.DC.Jobs.Model;
// using ESFA.DC.Jobs.Model.Enums;
// using ESFA.DC.JobScheduler.Interfaces.Models;
// using ESFA.DC.Logging.Interfaces;
// using ESFA.DC.Queueing.Interface.Configuration;
// using FluentAssertions;
// using Microsoft.VisualStudio.Threading;
// using Moq;
// using Xunit;

// namespace ESFA.DC.JobScheduler.Tests
// {
//    public class IlrMessageFactoryTests
//    {
//        [Fact]
//        public void CreateMessageParameters_Success()
//        {
//            var job = new FileUploadJob()
//            {
//                JobType = EnumJobType.IlrSubmission,
//                JobId = 10
//            };

// var factory = GetFactory(false, job);

// MessageParameters result = new JoinableTaskContext().Factory.Run(() => factory.CreateMessageParametersAsync(EnumJobType.IlrSubmission, It.IsAny<long>()));

// result.Should().NotBeNull();
//            result.JobType.Should().Be(EnumJobType.IlrSubmission);
//            result.JobContextMessage.JobId.Should().Be(10);
//            result.SubscriptionLabel.Should().Be("A");
//            result.JobContextMessage.Topics[0].SubscriptionName.Should().Be("A");
//            result.JobContextMessage.Topics[0].SubscriptionSqlFilterValue.Should().Be("B");
//            result.TopicParameters.ContainsKey("To").Should().Be(true);
//        }

// [Fact]
//        public async Task GenerateKeysAsync()
//        {
//            var job = new FileUploadJob()
//            {
//                JobType = EnumJobType.IlrSubmission,
//                JobId = 10,
//                Ukprn = 123456
//            };

// MessageFactory factory = GetFactory(false, job);

// MessageParameters result = await factory.CreateMessageParametersAsync(EnumJobType.IlrSubmission, It.IsAny<long>());

// result.JobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbers].Should()
//                .Be($"{job.Ukprn}/{job.JobId}/ValidationInvalidLearners.json");
//        }

// //[Theory]
//        //[InlineData(true)]
//        //[InlineData(false)]
//        //public void AddExtraKeys_Test(bool isFirstStage)
//        //{
//        //    var message = new JobContextMessage()
//        //    {
//        //        KeyValuePairs = new Dictionary<string, object>()
//        //    };

// //    var job = new FileUploadJob()
//        //    {
//        //        FileSize = 123,
//        //        Ukprn = 999,
//        //        JobId = 100,
//        //        StorageReference = "ref",
//        //        FileName = "filename.xml",
//        //        SubmittedBy = "test user",
//        //        DateTimeCreatedUtc = new DateTime(2018, 10, 10),
//        //        IsFirstStage = isFirstStage,
//        //    };

// //    message.KeyValuePairs.Add("Filename", job.FileName);

// //    var factory = GetFactory(isFirstStage, job);

// //    //factory.AddExtraKeys(message, job);

// //    message.KeyValuePairs[JobContextMessageKey.FileSizeInBytes].Should().Be(123);

// //    if (isFirstStage)
//        //    {
//        //        message.KeyValuePairs[JobContextMessageKey.PauseWhenFinished].Should().Be("1");
//        //    }
//        //    else
//        //    {
//        //        message.KeyValuePairs.ContainsKey(JobContextMessageKey.PauseWhenFinished).Should().Be(false);
//        //    }

// //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.InvalidLearnRefNumbers).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.ValidLearnRefNumbers).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.ValidationErrors).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.ValidationErrorLookups).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.FundingAlbOutput).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.FundingFm35Output).Should().BeTrue();
//        //    message.KeyValuePairs.ContainsKey(JobContextMessageKey.FundingFm25Output).Should().BeTrue();
//        //}

// private MessageFactory GetFactory(bool isFirstStage = true, FileUploadJob job = null)
//        {
//            var mockIFileUploadJobManager = new Mock<IFileUploadJobManager>();
//            mockIFileUploadJobManager.Setup(x => x.GetJobById(It.IsAny<long>())).ReturnsAsync(
//                job ?? new FileUploadJob
//                {
//                    IsFirstStage = isFirstStage,
//                    JobId = 10,
//                    Ukprn = 1000
//                });

// var mockTopicConfiguration = new Mock<ITopicConfiguration>();
//            mockTopicConfiguration.SetupGet(x => x.SubscriptionName).Returns("Validation");

// var jobTopicTaskService = new Mock<IJobTopicTaskService>();
//            jobTopicTaskService.Setup(x => x.GetTopicItems(It.IsAny<EnumJobType>(), It.IsAny<bool>(), default(CancellationToken))).ReturnsAsync(
//                new List<ITopicItem>()
//                {
//                    new TopicItem("A", "B", new List<ITaskItem>())
//                });

// var factory = new MessageFactory(null, mockIFileUploadJobManager.Object, null, jobTopicTaskService.Object);

// return factory;
//        }
//    }
// }
