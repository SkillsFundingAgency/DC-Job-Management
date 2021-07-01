using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services;
using ESFA.DC.PeriodEnd.Utils;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public sealed class PeriodEndOutputServiceTest
    {
        [Theory]
        [InlineData(JobStatusType.Ready, JobStatusType.Completed, JobStatusType.Completed, JobStatusType.Completed)]
        [InlineData(JobStatusType.Completed, JobStatusType.Ready, JobStatusType.Completed, JobStatusType.Completed)]
        [InlineData(JobStatusType.Completed, JobStatusType.Completed, JobStatusType.Ready, JobStatusType.Completed)]
        [InlineData(JobStatusType.Completed, JobStatusType.Completed, JobStatusType.Completed, JobStatusType.Ready)]
        [Trait("Category", "PeriodEnd")]
        public async Task Check_Critical_Path(JobStatusType mca, JobStatusType esf, JobStatusType apps, JobStatusType dc)
        {
            const int year = 1920;
            const int period = 5;
            const string collectionType = "ILR";

            PeriodEndState periodEndState = new PeriodEndState();
            PathPathItemsModel pathItemsModel = new PathPathItemsModel
            {
                Name = "Critical Path",
                PathId = 0,
                PathItems = new[]
                {
                    new PathItemModel
                    {
                        IsProviderReport = true,
                        PathItemId = PeriodEndPathItem.MCAReports,
                        PathItemJobs = new List<PathItemJobModel>
                        {
                            new PathItemJobModel
                            {
                                JobId = 1,
                                Ordinal = 1,
                                ProviderName = "123456",
                                Status = (int)mca
                            }
                        }
                    },
                    new PathItemModel
                    {
                        IsProviderReport = true,
                        PathItemId = PeriodEndPathItem.ESFSummarisation,
                        PathItemJobs = new List<PathItemJobModel>
                        {
                            new PathItemJobModel
                            {
                                JobId = 2,
                                Ordinal = 2,
                                ProviderName = "123456",
                                Status = (int)esf
                            }
                        }
                    },
                    new PathItemModel
                    {
                        IsProviderReport = true,
                        PathItemId = PeriodEndPathItem.AppSummarisation,
                        PathItemJobs = new List<PathItemJobModel>
                        {
                            new PathItemJobModel
                            {
                                JobId = 3,
                                Ordinal = 3,
                                ProviderName = "123456",
                                Status = (int)apps
                            }
                        }
                    },
                    new PathItemModel
                    {
                        IsProviderReport = true,
                        PathItemId = PeriodEndPathItem.DCSummarisation,
                        PathItemJobs = new List<PathItemJobModel>
                        {
                            new PathItemJobModel
                            {
                                JobId = 4,
                                Ordinal = 4,
                                ProviderName = "123456",
                                Status = (int)dc
                            }
                        }
                    }
                }
            };

            Mock<IPeriodEndRepository> periodEndRepositoryMock = new Mock<IPeriodEndRepository>();

            PeriodEndOutputService periodEndOutputService = new PeriodEndOutputService(periodEndRepositoryMock.Object, new Mock<IPeriodEndEmailService>().Object, new Mock<ILogger>().Object);
            await periodEndOutputService.CheckCriticalPath(periodEndState, pathItemsModel, year, period, collectionType);

            periodEndRepositoryMock.Verify(x => x.McaReportsReadyAsync(year, period, collectionType, CancellationToken.None), Times.Exactly(mca == JobStatusType.Completed ? 1 : 0));
            periodEndRepositoryMock.Verify(x => x.EsfSummarisationReadyAsync(year, period, collectionType, CancellationToken.None), Times.Exactly(esf == JobStatusType.Completed ? 1 : 0));
            periodEndRepositoryMock.Verify(x => x.AppsSummarisationReadyAsync(year, period, collectionType, CancellationToken.None), Times.Exactly(apps == JobStatusType.Completed ? 1 : 0));
            periodEndRepositoryMock.Verify(x => x.DcSummarisationReadyAsync(year, period, collectionType, CancellationToken.None), Times.Exactly(dc == JobStatusType.Completed ? 1 : 0));
        }

        [Theory]
        [MemberData(nameof(PeriodEndProviderReportReadyData.Data), MemberType = typeof(PeriodEndProviderReportReadyData))]
        [Trait("Category", "PeriodEnd")]
        public async Task Check_Provider_Report_Jobs(bool shouldReportsBeReady, List<PathPathItemsModel> pathItemModels)
        {
            const int year = 1920;
            const int period = 5;
            const string collectionType = "ILR";

            Mock<IPeriodEndRepository> periodEndRepositoryMock = new Mock<IPeriodEndRepository>();

            PeriodEndOutputService periodEndOutputService = new PeriodEndOutputService(periodEndRepositoryMock.Object, new Mock<IPeriodEndEmailService>().Object, new Mock<ILogger>().Object);
            await periodEndOutputService.CheckProviderReportJobs(pathItemModels, year, period, collectionType);

            periodEndRepositoryMock.Verify(x => x.ProviderReportsReadyAsync(year, period, collectionType, CancellationToken.None), Times.Exactly(shouldReportsBeReady ? 1 : 0));
        }
    }
}