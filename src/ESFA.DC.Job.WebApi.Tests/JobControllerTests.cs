// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Threading.Tasks;
// using ESFA.DC.DateTimeProvider.Interface;
// using ESFA.DC.Job.WebApi.Controllers;
// using ESFA.DC.JobQueueManager.Interfaces;
// using ESFA.DC.Jobs.Model;
// using ESFA.DC.Jobs.Model.Enums;
// using ESFA.DC.Logging.Interfaces;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;

// namespace ESFA.DC.Job.WebApi.Tests
// {
//    public class JobControllerTests
//    {
//        [Fact]
//        public void GetAllJobs_EmptyList_Test()
//        {
//            var jobqueServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueServiceMock.Setup(x => x.GetAllJobs()).ReturnsAsync(new List<FileUploadJob>());
//            var controller = GetController(null, jobqueServiceMock.Object);

// var result = (OkObjectResult)controller.Get().Result;
//            result.StatusCode.Should().Be(200);
//            result.Value.Should().BeAssignableTo<List<FileUploadJob>>();
//        }

// [Fact]
//        public void GetAllJobs_OrderedList_Test()
//        {
//            var jobqueServiceMock = new Mock<IFileUploadJobManager>();

// var jobs = new List<FileUploadJob>()
//            {
//                new FileUploadJob()
//                {
//                    JobId = 1,
//                    Status = JobStatusType.Completed,
//                    Priority = 3,
//                },
//                new FileUploadJob()
//                {
//                    JobId = 2,
//                    Status = JobStatusType.Ready,
//                    Priority = 5,
//                },
//                new FileUploadJob()
//                {
//                    JobId = 3,
//                    Status = JobStatusType.Ready,
//                    Priority = 1,
//                },
//                new FileUploadJob()
//                {
//                    JobId = 4,
//                    Status = JobStatusType.Ready,
//                    Priority = 5,
//                },
//                new FileUploadJob()
//                {
//                    JobId = 4,
//                    Status = JobStatusType.Completed,
//                    Priority = 100,
//                },
//            };

// jobqueServiceMock.Setup(x => x.GetAllJobs()).ReturnsAsync(jobs);
//            var controller = GetController(null, jobqueServiceMock.Object);

// var result = (OkObjectResult)controller.Get().Result;
//            result.StatusCode.Should().Be(200);

// var outputJobs = (List<FileUploadJob>)result.Value;
//            outputJobs.Count.Should().Be(5);
//            outputJobs.First().JobId.Should().Be(2);
//        }

// [Fact]
//        public void GetJobByJobId_ZeroJobIdValue_Test()
//        {
//            var jobqueMetaServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueMetaServiceMock.Setup(x => x.GetJobById(2)).ReturnsAsync(It.IsAny<FileUploadJob>());

// var controller = GetController(null, jobqueMetaServiceMock.Object);

// var result = (BadRequestResult)controller.GetById(1000, 0).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void GetJobByJobId_UkprnZeroValue_Test()
//        {
//            var controller = GetController();

// var result = (BadRequestResult)controller.GetById(0, 100).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void GetJobByJobId_Success_Test()
//        {
//            var jobqueMetaServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueMetaServiceMock.Setup(x => x.GetJobById(2)).ReturnsAsync(new FileUploadJob()
//            {
//                JobId = 2,
//                Status = JobStatusType.Ready,
//                Priority = 5,
//                Ukprn = 1000,
//            });

// var controller = GetController(null, jobqueMetaServiceMock.Object);

// var result = (OkObjectResult)controller.GetById(1000, 2).Result;
//            result.StatusCode.Should().Be(200);

// var outputJob = (FileUploadJob)result.Value;
//            outputJob.JobId.Should().Be(2);
//            outputJob.Status.Should().Be((short)JobStatusType.Ready);
//            outputJob.Priority.Should().Be(5);
//            outputJob.Ukprn.Should().Be(1000);
//        }

// [Fact]
//        public void GetJobsForUkprn_UkprnZeroValue_Test()
//        {
//            var controller = GetController();

// var result = (BadRequestResult)controller.GetForUkprn(0).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void GetJobsForUkprn_Success_Test()
//        {
//            var jobqueServiceMock = new Mock<IFileUploadJobManager>();
//            var jobs = new List<FileUploadJob>()
//            {
//                new FileUploadJob()
//                {
//                    JobId = 1,
//                    Status = JobStatusType.Completed,
//                    Priority = 3,
//                },
//                new FileUploadJob()
//                {
//                    JobId = 2,
//                    Status = JobStatusType.Ready,
//                    Priority = 5,
//                },
//            };

// jobqueServiceMock.Setup(x => x.GetJobsByUkprn(1000)).ReturnsAsync(jobs);
//            var controller = GetController(null, jobqueServiceMock.Object);

// var result = (OkObjectResult)controller.GetForUkprn(1000).Result;
//            result.StatusCode.Should().Be(200);

// var outputJobs = (List<FileUploadJob>)result.Value;
//            outputJobs.Count.Should().Be(2);
//        }

// [Fact]
//        public void PostJob_Null_Test()
//        {
//            var controller = GetController();

// var result = (BadRequestResult)controller.Post((FileUploadJob)null).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
// #pragma warning disable SA1600 // Elements should be documented
//        public void PostJob_NewJob_Success_Test()
// #pragma warning restore SA1600 // Elements should be documented
//        {
//            var job = new FileUploadJob
//            {
//                Status = JobStatusType.Ready,
//                CreatedBy = "test",
//                JobType = EnumJobType.IlrSubmission,
//                JobId = 0,
//                Ukprn = 1000,
//                FileName = "test",
//            };

// var jobqueMetaServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueMetaServiceMock.Setup(x => x.AddJob(job)).ReturnsAsync(1);

// var controller = GetController(null, jobqueMetaServiceMock.Object);

// var result = (OkObjectResult)controller.Post(job).Result;
//            result.StatusCode.Should().Be(200);
//            result.Value.Should().Be(1);
//        }

// [Fact]
//        public void PostJob_NewJob_SaveFailed_Test()
//        {
//            var job = new FileUploadJob()
//            {
//                Status = JobStatusType.Ready,
//                JobType = EnumJobType.IlrSubmission,
//            };
//            var jobqueServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueServiceMock.Setup(x => x.AddJob(It.IsAny<FileUploadJob>())).ReturnsAsync(0);

// var controller = GetController(null, jobqueServiceMock.Object);

// var result = (BadRequestResult)controller.Post(job).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void PostJob_NewJob_InvalidStatus_Failed_Test()
//        {
//            var job = new FileUploadJob();
//            var controller = GetController();

// var result = (BadRequestObjectResult)controller.Post(job).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void PostJob_NewJob_InvalidJobType_Failed_Test()
//        {
//            var job = new FileUploadJob
//            {
//                Status = JobStatusType.Ready,
//            };

// var controller = GetController();

// var result = (BadRequestObjectResult)controller.Post(job).Result;
//            result.StatusCode.Should().Be(400);
//        }

// [Theory]
//        [InlineData(JobStatusType.MovedForProcessing)]
//        [InlineData(JobStatusType.Processing)]
//        [InlineData(JobStatusType.Completed)]
//        [InlineData(JobStatusType.Failed)]
//        public void PostJob_UpdateJob_InvalidStatus_Failed_Test(JobStatusType status)
//        {
//            var job = new FileUploadJob()
//            {
//                JobId = 100,
//                Status = status,
//                Ukprn = 1000,
//            };

// var controller = GetController();

// var result = controller.Post(job).Result;
//            result.Should().BeAssignableTo<BadRequestObjectResult>();
//            ((BadRequestObjectResult)result).StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void PostJob_UpdateJobStatus_Failed_NullDtoTest()
//        {
//            var controller = GetController();

// var result = controller.Post((JobStatusDto)null).Result;
//            result.Should().BeAssignableTo<BadRequestResult>();
//            ((BadRequestResult)result).StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void PostJob_UpdateJobStatus_Failed_ZeroJobIdTest()
//        {
//            var controller = GetController();

// var result = controller.Post(new JobStatusDto()).Result;
//            result.Should().BeAssignableTo<BadRequestResult>();
//            ((BadRequestResult)result).StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void PostJob_UpdateJobStatus_Success_Test()
//        {
//            var jobqueServiceMock = new Mock<IJobManager>();
//            jobqueServiceMock.Setup(x => x.UpdateJobStatus(It.IsAny<long>(), JobStatusType.Ready)).ReturnsAsync(true);
//            jobqueServiceMock.Setup(x => x.GetJobById(100)).ReturnsAsync(new Jobs.Model.Job() { JobId = 100 });

// var controller = GetController(jobqueServiceMock.Object);

// var result = controller.Post(new JobStatusDto()
//            {
//                JobStatus = (int)JobStatusType.Ready,
//                JobId = 100,
//            }).Result;
//            result.Should().BeAssignableTo<OkResult>();
//            ((OkResult)result).StatusCode.Should().Be(200);
//        }

// [Fact]
//        public void Delete_ZeroId_Failed_Test()
//        {
//            var controller = GetController();

// var result = controller.Delete(0).Result;
//            result.Should().BeAssignableTo<BadRequestResult>();
//            ((BadRequestResult)result).StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void Delete_Success_Test()
//        {
//            var jobqueServiceMock = new Mock<IJobManager>();
//            jobqueServiceMock.Setup(x => x.RemoveJobFromQueue(100)).Returns(Task.CompletedTask);

// var mockLogger = new Mock<ILogger>();
//            var controller = new JobController(jobqueServiceMock.Object, mockLogger.Object, new Mock<IDateTimeProvider>().Object, null, null, null, null);

// var result = controller.Delete(100).Result;
//            result.Should().BeAssignableTo<OkResult>();
//            ((OkResult)result).StatusCode.Should().Be(200);
//        }

// [Fact]
//        public void GetJobByStatus_FailJobDontExist_Test()
//        {
//            var jobqueServiceMock = new Mock<IFileUploadJobManager>();
//            jobqueServiceMock.Setup(x => x.GetJobById(1)).ReturnsAsync(new FileUploadJob()
//            {
//                JobId = 1,
//                Status = JobStatusType.Ready,
//                Priority = 5,
//            });
//            var controller = GetController(null, jobqueServiceMock.Object);

// var result = (BadRequestResult)controller.GetStatus(10).GetAwaiter().GetResult();
//            result.StatusCode.Should().Be(400);
//        }

// [Fact]
//        public void GetJobByStatus_Success_Test()
//        {
//            var jobqueServiceMock = new Mock<IJobManager>();
//            jobqueServiceMock.Setup(x => x.GetJobById(2)).ReturnsAsync(new Jobs.Model.Job()
//            {
//                JobId = 2,
//                Status = JobStatusType.Ready,
//                Priority = 5,
//            });
//            var controller = GetController(jobqueServiceMock.Object);

// var result = (OkObjectResult)controller.GetStatus(2).Result;
//            result.StatusCode.Should().Be(200);
//            result.Value.Should().BeAssignableTo<JobStatusType>();
//            result.Value.Should().Be(JobStatusType.Ready);
//        }

// private JobController GetController(IJobManager jobManager = null, IFileUploadJobManager fileUploadJobManager = null)
//        {
//            if (jobManager == null)
//            {
//                jobManager = new Mock<IJobManager>().Object;
//            }

// if (fileUploadJobManager == null)
//            {
//                fileUploadJobManager = new Mock<IFileUploadJobManager>().Object;
//            }

// var mockLogger = new Mock<ILogger>();

// return new JobController(jobManager, mockLogger.Object, new Mock<IDateTimeProvider>().Object, fileUploadJobManager, null, null, null);
//        }
//    }
// }