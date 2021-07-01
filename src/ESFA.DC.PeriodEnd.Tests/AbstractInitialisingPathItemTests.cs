using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart1Path;
using ESFA.DC.PeriodEnd.Utils;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class AbstractInitialisingPathItemTests
    {
        [Fact]
        public async Task ExecuteAsync_Calls_StateService_SavePathItem()
        {
            var collectionYear = 2021;
            var period = 1;
            var ordinal = 0;

            var pathItemParams = new PathItemParams
            {
                Ordinal = ordinal,
                Period = period,
                CollectionYear = collectionYear
            };

            var pathItem = new PathItemModel
            {
                PathId = Convert.ToInt32(PeriodEndPath.FCSHandOverPath1),
                Ordinal = ordinal,
                HasJobs = false,
                IsPausing = true,
                PathItemJobs = new List<PathItemJobModel>()
            };

            var logger = new Mock<ILogger>();
            var jobManager = new Mock<IFileUploadJobManager>();
            var jobFactory = new Mock<IPeriodEndJobFactory>();
            var returnsFactory = new Mock<IPathItemReturnFactory>();

            var stateService = new Mock<IStateService>();
            stateService.Setup(m => m.SavePathItem(PathItemMatches(pathItem), collectionYear, period));

            var service = new FCSPart1InitialBlockingItem(logger.Object, jobManager.Object, jobFactory.Object, returnsFactory.Object, stateService.Object);

            await service.ExecuteAsync(pathItemParams);

            stateService.Verify(m => m.SavePathItem(PathItemMatches(pathItem), collectionYear, period), Times.Once);
        }

        public PathItemModel PathItemMatches(PathItemModel template)
        {
            return Match.Create<PathItemModel>(m => m.PathId == template.PathId
                                                    && m.Ordinal == template.Ordinal
                                                    && m.HasJobs == template.HasJobs
                                                    && m.IsPausing == template.IsPausing);
        }
    }
}