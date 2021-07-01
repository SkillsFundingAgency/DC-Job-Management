using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Processing.JobsFailedToday;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
using ESFA.DC.Jobs.Model.Processing.JobsQueued;
using ESFA.DC.Jobs.Model.Processing.JobsSlowFile;
using ESFA.DC.Jobs.Model.Processing.JobsSubmitted;
using ESFA.DC.ReferenceData.Organisations.Model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;
using ReturnPeriod = ESFA.DC.CollectionsManagement.Models.ReturnPeriod;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class JobProcessingServiceTests : AbstractBaseJobQueueManagerTests
    {
        [Fact]
        public async Task GetJobsThatAreProcessingTest()
        {
            var jobQueryService = new Mock<IJobProcessingService>();
            var jobsThatAreProcessing = new JobsProcessingModel();
            jobsThatAreProcessing.Jobs.Add(2019, new List<JobProcessingLookupModel>
            {
                new JobProcessingLookupModel
                {
                    ProviderName = "A COLLEGE", DateDifferSecond = 47, TimeTakenSecond = 492752, Ukprn = 100001
                },
                new JobProcessingLookupModel
                {
                    ProviderName = "B COLLEGE", DateDifferSecond = 6, TimeTakenSecond = 419366, Ukprn = 100002
                },
                new JobProcessingLookupModel
                {
                    ProviderName = "C COLLEGE", DateDifferSecond = 12, TimeTakenSecond = 85751, Ukprn = 100003
                },
            });

            jobQueryService.Setup(x => x.GetJobsThatAreProcessing(It.IsAny<DateTime>()))
                .ReturnsAsync(() => jobsThatAreProcessing);

            var jobProcessing = await jobQueryService.Object.GetJobsThatAreProcessing(DateTime.Now);

            jobProcessing.Should().BeOfType(typeof(JobsProcessingModel));

            jobProcessing.Jobs.Should().BeOfType(typeof(Dictionary<int, List<JobProcessingLookupModel>>));

            jobProcessing.Jobs.Count.Should().Be(1);

            var jobsByYear = jobProcessing.Jobs.FirstOrDefault().Value;
            jobsByYear.Count.Should().Be(3);

            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals(string.Empty)).Should().BeNull();
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"))?.Ukprn.Should().Be(100001);
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"))?.Ukprn.Should().Be(100002);
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"))?.Ukprn.Should().Be(100003);

            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(0)).Should().BeNull();
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100001))?.ProviderName.Should().Be("A COLLEGE");
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100002))?.ProviderName.Should().Be("B COLLEGE");
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100003))?.ProviderName.Should().Be("C COLLEGE");

            var bow_School = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"));
            bow_School.Should().BeOfType(typeof(JobProcessingLookupModel));
            bow_School.TimeTakenSecond.Should().Be(492752);
            bow_School.DateDifferSecond.Should().Be(47);
            bow_School.TimeTaken.Should().Be("5 days 16 hours 52 minutes 32 seconds");
            bow_School.AverageProcessingTime.Should().Be("47 seconds");

            var the_Blackpool_Sixth_Form_College =
                jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"));
            the_Blackpool_Sixth_Form_College.Should().BeOfType(typeof(JobProcessingLookupModel));
            the_Blackpool_Sixth_Form_College?.TimeTakenSecond.Should().Be(419366);
            the_Blackpool_Sixth_Form_College?.DateDifferSecond.Should().Be(6);
            the_Blackpool_Sixth_Form_College?.TimeTaken.Should().Be("4 days 20 hours 29 minutes 26 seconds");
            the_Blackpool_Sixth_Form_College?.AverageProcessingTime.Should().Be("6 seconds");

            var writtle_University_College = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"));
            writtle_University_College.Should().BeOfType(typeof(JobProcessingLookupModel));
            writtle_University_College?.TimeTakenSecond.Should().Be(85751);
            writtle_University_College?.DateDifferSecond.Should().Be(12);
            writtle_University_College?.TimeTaken.Should().Be("23 hours 49 minutes 11 seconds");
            writtle_University_College?.AverageProcessingTime.Should().Be("12 seconds");
        }

        [Fact]
        public async Task GetJobsThatAreQueuedTest()
        {
            var jobQueryService = new Mock<IJobProcessingService>();

            var jobsThatAreQueued = new JobsQueuedModel();
            jobsThatAreQueued.Jobs.Add(2019, new List<JobQueuedLookupModel>
            {
                new JobQueuedLookupModel
                {
                    ProviderName = "A COLLEGE", Status = 1, TimeInQueueSecond = 492752, Ukprn = 100001
                },
                new JobQueuedLookupModel
                {
                    ProviderName = "B COLLEGE", Status = 1, TimeInQueueSecond = 419366, Ukprn = 100002
                },
                new JobQueuedLookupModel
                {
                    ProviderName = "C COLLEGE", Status = 1, TimeInQueueSecond = 85751, Ukprn = 100003
                },
            });

            jobQueryService.Setup(x => x.GetJobsThatAreQueued(It.IsAny<DateTime>()))
                .ReturnsAsync(() => jobsThatAreQueued);

            var jobQueued = await jobQueryService.Object.GetJobsThatAreQueued(DateTime.Now);

            jobQueued.Should().BeOfType(typeof(JobsQueuedModel));

            jobQueued.Jobs.Should().BeOfType(typeof(Dictionary<int, List<JobQueuedLookupModel>>));

            jobQueued.Jobs.Count.Should().Be(1);
            var jobsByYear = jobQueued.Jobs.FirstOrDefault().Value;
            jobsByYear.Count.Should().Be(3);

            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals(string.Empty)).Should().BeNull();
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"))?.Ukprn.Should().Be(100001);
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"))?.Ukprn.Should().Be(100002);
            jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"))?.Ukprn.Should().Be(100003);

            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(0)).Should().BeNull();
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100001))?.ProviderName.Should().Be("A COLLEGE");
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100002))?.ProviderName.Should().Be("B COLLEGE");
            jobsByYear.FirstOrDefault(x => x.Ukprn.Equals(100003))?.ProviderName.Should().Be("C COLLEGE");

            var bow_School = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"));
            bow_School.Should().BeOfType(typeof(JobQueuedLookupModel));
            bow_School?.TimeInQueueSecond.Should().Be(492752);
            bow_School?.TimeInQueue.Should().Be("5 days 16 hours 52 minutes 32 seconds");

            var the_Blackpool_Sixth_Form_College =
                jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"));
            the_Blackpool_Sixth_Form_College.Should().BeOfType(typeof(JobQueuedLookupModel));
            the_Blackpool_Sixth_Form_College?.TimeInQueueSecond.Should().Be(419366);
            the_Blackpool_Sixth_Form_College?.TimeInQueue.Should().Be("4 days 20 hours 29 minutes 26 seconds");

            var writtle_University_College = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"));
            writtle_University_College.Should().BeOfType(typeof(JobQueuedLookupModel));
            writtle_University_College?.TimeInQueueSecond.Should().Be(85751);
            writtle_University_College?.TimeInQueue.Should().Be("23 hours 49 minutes 11 seconds");

            jobsByYear.Where(x => x.Status == 1)?.Count().Should().Be(3);
        }

        [Fact]
        public async Task GetJobsThatAreSubmittedTest()
        {
            var jobQueryService = new Mock<IJobProcessingService>();
            var now = DateTime.Now;
            var jobsThatAreSubmitted = new JobsSubmittedModel();
            jobsThatAreSubmitted.Jobs.Add(2019, new List<JobSubmittedLookupModel>
            {
                new JobSubmittedLookupModel
                {
                    ProviderName = "A COLLEGE", Ukprn = 100001, CreatedDate = now, FileName = "File1.xml",
                    Status = 3, StatusDescription = "Processing"
                },
                new JobSubmittedLookupModel
                {
                    ProviderName = "B COLLEGE", Ukprn = 100002, CreatedDate = now.AddMinutes(5),
                    FileName = "File2.xml", Status = 2, StatusDescription = "Moved For Processing"
                },
                new JobSubmittedLookupModel
                {
                    ProviderName = "C COLLEGE", Ukprn = 100003, CreatedDate = now.AddMinutes(10),
                    FileName = "File3.xml", Status = 4, StatusDescription = "Completed"
                },
                new JobSubmittedLookupModel
                {
                    ProviderName = "D COLLEGE", Ukprn = 100004, CreatedDate = now.AddDays(-1),
                    FileName = "File4.xml", Status = 2, StatusDescription = "Moved For Processing"
                },
                new JobSubmittedLookupModel
                {
                    ProviderName = "E COLLEGE", Ukprn = 100005, CreatedDate = now.AddDays(-2),
                    FileName = "File5.xml", Status = 3, StatusDescription = "Processing"
                },
            });

            jobQueryService.Setup(x => x.GetJobsThatAreSubmitted(It.IsAny<DateTime>()))
                .ReturnsAsync(() => jobsThatAreSubmitted);

            var jobSubmitted = await jobQueryService.Object.GetJobsThatAreSubmitted(now.Date);

            jobSubmitted.Should().BeOfType(typeof(JobsSubmittedModel));

            jobSubmitted.Jobs.Should().BeOfType(typeof(Dictionary<int, List<JobSubmittedLookupModel>>));

            jobSubmitted.Jobs.Count.Should().Be(1);

            var jobsByYear = jobSubmitted.Jobs.FirstOrDefault().Value;
            jobsByYear.Count.Should().Be(5);

            jobsByYear.Where(x => x.CreatedDate >= now.Date)?.Count().Should().Be(3);

            jobsByYear.Where(x => x.CreatedDate < now.Date)?.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetJobsThatAreFailedTodayTest()
        {
            var jobQueryService = new Mock<IJobProcessingService>();
            var now = DateTime.Now;
            var jobsThatHaveFailedToday = new JobsFailedTodayModel();
            jobsThatHaveFailedToday.Jobs.Add(2019, new List<JobFailedTodayLookupModel>
            {
                new JobFailedTodayLookupModel
                {
                    ProviderName = "A COLLEGE", Ukprn = 100001, FailedAt = now, FileName = "File1.xml", ProcessingTimeBeforeFailureSecond = 492752,
                },
                new JobFailedTodayLookupModel
                {
                    ProviderName = "B COLLEGE", Ukprn = 100002, FailedAt = now, FileName = "File2.xml", ProcessingTimeBeforeFailureSecond = 419366,
                },
                new JobFailedTodayLookupModel
                {
                    ProviderName = "C COLLEGE", Ukprn = 100003, FailedAt = now, FileName = "File3.xml", ProcessingTimeBeforeFailureSecond = 85751,
                },
            });

            jobQueryService.Setup(x => x.GetJobsThatAreFailedToday(It.IsAny<DateTime>()))
                .ReturnsAsync(() => jobsThatHaveFailedToday);

            var jobFailedToday = await jobQueryService.Object.GetJobsThatAreFailedToday(now.Date);

            var jobsByYear = jobFailedToday.Jobs.FirstOrDefault().Value;
            jobsByYear.Count.Should().Be(3);

            var bow_School = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"));
            bow_School.Should().BeOfType(typeof(JobFailedTodayLookupModel));
            bow_School?.ProcessingTimeBeforeFailureSecond.Should().Be(492752);
            bow_School?.ProcessingTimeBeforeFailure.Should().Be("5 days 16 hours 52 minutes 32 seconds");

            var the_Blackpool_Sixth_Form_College =
                jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"));
            the_Blackpool_Sixth_Form_College.Should().BeOfType(typeof(JobFailedTodayLookupModel));
            the_Blackpool_Sixth_Form_College?.ProcessingTimeBeforeFailureSecond.Should().Be(419366);
            the_Blackpool_Sixth_Form_College?.ProcessingTimeBeforeFailure.Should()
                .Be("4 days 20 hours 29 minutes 26 seconds");

            var writtle_University_College =
                jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"));
            writtle_University_College.Should().BeOfType(typeof(JobFailedTodayLookupModel));
            writtle_University_College?.ProcessingTimeBeforeFailureSecond.Should().Be(85751);
            writtle_University_College?.ProcessingTimeBeforeFailure.Should().Be("23 hours 49 minutes 11 seconds");
        }

        [Fact]
        public async Task GetJobsThatAreSlowFileTest()
        {
            var jobQueryService = new Mock<IJobProcessingService>();
            var now = DateTime.Now;
            var jobsThatAreSlowFiles = new JobsSlowFileModel();
            jobsThatAreSlowFiles.Jobs.Add(2019, new List<JobSlowFileLookupModel>
            {
                new JobSlowFileLookupModel
                {
                    ProviderName = "A COLLEGE", Ukprn = 100001, FileName = "File1.xml", TimeTakenSecond = 492752, AverageTimeSecond = 47,
                },
                new JobSlowFileLookupModel
                {
                    ProviderName = "B COLLEGE", Ukprn = 100002, FileName = "File2.xml", TimeTakenSecond = 419366, AverageTimeSecond = 6,
                },
                new JobSlowFileLookupModel
                {
                    ProviderName = "C COLLEGE", Ukprn = 100003, FileName = "File3.xml", TimeTakenSecond = 85751, AverageTimeSecond = 12,
                },
            });

            jobQueryService.Setup(x => x.GetJobsThatAreSlowFile(It.IsAny<DateTime>()))
                .ReturnsAsync(() => jobsThatAreSlowFiles);

            var jobSlowFile = await jobQueryService.Object.GetJobsThatAreSlowFile(now);

            var jobsByYear = jobSlowFile.Jobs.FirstOrDefault().Value;
            jobsByYear.Count.Should().Be(3);

            var bow_School = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("A COLLEGE"));
            bow_School.Should().BeOfType(typeof(JobSlowFileLookupModel));
            bow_School?.TimeTakenSecond.Should().Be(492752);
            bow_School?.TimeTaken.Should().Be("5 days 16 hours 52 minutes 32 seconds");
            bow_School?.AverageTimeSecond.Should().Be(47);
            bow_School?.AverageTime.Should().Be("47 seconds");

            var the_Blackpool_Sixth_Form_College =
                jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("B COLLEGE"));
            the_Blackpool_Sixth_Form_College.Should().BeOfType(typeof(JobSlowFileLookupModel));
            the_Blackpool_Sixth_Form_College?.TimeTakenSecond.Should().Be(419366);
            the_Blackpool_Sixth_Form_College?.TimeTaken.Should().Be("4 days 20 hours 29 minutes 26 seconds");
            the_Blackpool_Sixth_Form_College?.AverageTimeSecond.Should().Be(6);
            the_Blackpool_Sixth_Form_College?.AverageTime.Should().Be("6 seconds");

            var writtle_University_College = jobsByYear.FirstOrDefault(x => x.ProviderName.Equals("C COLLEGE"));
            writtle_University_College.Should().BeOfType(typeof(JobSlowFileLookupModel));
            writtle_University_College?.TimeTakenSecond.Should().Be(85751);
            writtle_University_College?.TimeTaken.Should().Be("23 hours 49 minutes 11 seconds");
            writtle_University_College?.AverageTimeSecond.Should().Be(12);
            writtle_University_College?.AverageTime.Should().Be("12 seconds");
        }

        [Fact]
        public async Task GetPaginatedJobsInDetailForCurrentOrClosedPeriod_Success()
        {
            var dateTime = new DateTime(2020, 01, 10, 10, 20, 30);
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);

            var returnCalendarServiceMock = new Mock<IReturnCalendarService>();
            returnCalendarServiceMock.Setup(x => x.GetOpenPeriodsAsync(null, "ILR")).ReturnsAsync(
                new List<ReturnPeriod>()
                {
                    new ReturnPeriod()
                    {
                        StartDateTimeUtc = dateTime,
                        EndDateTimeUtc = dateTime.AddDays(28),
                    },
                });

            IContainer container = GetRegistrations(dateTimeProvider: dateTimeProviderMock.Object, returnCalendarService: returnCalendarServiceMock.Object);

            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    SetupDatabaseForJobs(context);
                    SetupCollections(context, CollectionConstants.ILR2021, 1);
                    SetupCollections(context, "EAS1920", 2);
                    SetupCollections(context, "ESFR2", 3);
                }

                var orgContextOptions = scope.Resolve<DbContextOptions<OrganisationsContext>>();
                using (var context = new OrganisationsContext(orgContextOptions))
                {
                    context.Database.EnsureCreated();

                    context.OrgDetails.Add(new OrgDetail()
                    {
                        Ukprn = 10000116,
                        Name = "provider1",
                        CreatedBy = "test",
                        CreatedOn = dateTime,
                        ModifiedBy = "test1",
                        ModifiedOn = dateTime,
                        UkprnNavigation = new MasterOrganisation()
                        {
                            Ukprn = 10000116,
                        },
                    });
                    context.OrgDetails.Add(new OrgDetail()
                    {
                        Ukprn = 10000117,
                        Name = "provider2",
                        CreatedBy = "test",
                        CreatedOn = dateTime,
                        ModifiedBy = "test1",
                        ModifiedOn = dateTime,
                        UkprnNavigation = new MasterOrganisation()
                        {
                            Ukprn = 10000117,
                        },
                    });
                    context.SaveChanges();
                }

                IFileUploadJobManager uploadManager = scope.Resolve<IFileUploadJobManager>();
                IJobManager jobManager = scope.Resolve<IJobManager>();

                dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime.AddMinutes(1));
                await uploadManager.AddJob(new FileUploadJob()
                {
                    JobId = 1,
                    Ukprn = 10000116,
                    FileName = "esf.csv",
                    CollectionName = "ESFR2",
                    Status = JobStatusType.Completed,
                    PeriodNumber = 1,
                });

                dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime.AddMinutes(1).AddMilliseconds(1000));
                await jobManager.UpdateJobStatus(1, JobStatusType.Completed);

                dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime.AddMinutes(2));
                await uploadManager.AddJob(new FileUploadJob()
                {
                    JobId = 2,
                    Ukprn = 10000117,
                    FileName = "eas.csv",
                    CollectionName = "EAS1920",
                    PeriodNumber = 1,
                });

                dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime.AddMinutes(2).AddMilliseconds(1000));
                await jobManager.UpdateJobStatus(2, JobStatusType.Completed);

                var queryService = scope.Resolve<IJobProcessingService>();
                var result = await queryService.GetPaginatedJobsInDetailForCurrentOrClosedPeriod((short)JobStatusType.Completed);

                result.Should().NotBeNull();
                result.TotalItems.Should().Be(2);
                result.PageNumber.Should().Be(1);
                result.List.Count.Should().Be(2);

                var data = result.List;

                data.Single(x => x.CollectionName == "ESFR2" && x.JobId == 1 && x.FileName == "esf.csv" && x.ProviderName == "provider1" && x.ProcessingTimeMilliSeconds > 0).Should().NotBeNull();
                data.Single(x => x.CollectionName == "EAS1920" && x.JobId == 2 && x.FileName == "eas.csv" && x.ProviderName == "provider2" && x.ProcessingTimeMilliSeconds > 0).Should().NotBeNull();
            }
        }
    }
}