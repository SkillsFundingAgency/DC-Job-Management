using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Controllers;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ESFA.DC.Job.WebApi.Tests
{
    public class OrganisationControllerTests
    {
        [Fact]
        public void GetSearchResults_NullSearchTerm()
        {
            var orgController = new OrganisationController(null, null, null, null, null, null);
            orgController.SearchProvidersAsync(CancellationToken.None, null).Result.Should().BeNull();
        }

        [Fact]
        public void GetSearchResults_Name_Test()
        {
            var orgnisationServiceMock = new Mock<IOrganisationService>();
            var jobqueryServiceMock = new Mock<IJobQueryService>();
            var reportArchiveServiceMock = new Mock<IReportsArchiveService>();

            orgnisationServiceMock.Setup(x => x.SearchProvidersInPimsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(ProvidersList());

            orgnisationServiceMock.Setup(x => x.GetProvidersWithCollectionAssignmentAsync(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<long>() { 10000001, 10000002, 10000003 });

            orgnisationServiceMock.Setup(x => x.GetProvidersWithFundingClaims(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<long>());

            jobqueryServiceMock.Setup(x => x.GetLatestJobByUkprnAsync(It.IsAny<long[]>(), CancellationToken.None))
                .ReturnsAsync(new List<ProviderLatestSubmission>());

            var orgController = new OrganisationController(orgnisationServiceMock.Object, null, jobqueryServiceMock.Object, null, null, reportArchiveServiceMock.Object);
            var result = orgController.SearchProvidersAsync(CancellationToken.None, "Org").Result;
            result.Count().Should().Be(3);
        }

        // [Fact]
        // public void GetSearchResults_Ukprn_Test()
        // {
        //    var orgnisationServiceMock = new Mock<IOrganisationService>();
        //    var jobQueryServiceMock = new Mock<IJobQueryService>();

        // jobQueryServiceMock.Setup(x => x.GetLatestJobByUkprn(new long[] { 1000 }))
        //        .ReturnsAsync(new List<ProviderLatestSubmission>()
        //        {
        //            new ProviderLatestSubmission()
        //            {
        //                Ukprn = 1000,
        //                LastSubmittedBy = "test",
        //                LastSubmittedDateUtc = new DateTime(2018, 10, 10),
        //                CollectionName = "ILR1819",
        //            },
        //            new ProviderLatestSubmission()
        //            {
        //                Ukprn = 1000,
        //                LastSubmittedBy = "test",
        //                LastSubmittedDateUtc = new DateTime(2018, 10, 12),
        //                CollectionName = "EAS",
        //            },
        //        });

        // var orgController = new OrganisationController(orgnisationServiceMock.Object, GetContext(), It.IsAny<IFileUploadJobManager>(), null, jobQueryServiceMock.Object);
        //    var result = orgController.SearchProviders("1000").Result;
        //    result.Count().Should().Be(1);
        //    result.Any(x =>
        //            x.Ukprn == 1000 &&
        //            x.Name.Equals("Org1", StringComparison.CurrentCultureIgnoreCase) &&
        //            x.ProviderLatestSubmissions.Any(y =>
        //                y.LastSubmittedBy.Equals("test", StringComparison.CurrentCultureIgnoreCase) &&
        //                y.LastSubmittedDateUtc == new DateTime(2018, 10, 10) &&
        //                y.CollectionName == "ILR1819"))
        //        .Should()
        //        .BeTrue();

        // result.Any(x =>
        //            x.Ukprn == 1000 &&
        //            x.Name.Equals("Org1", StringComparison.CurrentCultureIgnoreCase) &&
        //            x.ProviderLatestSubmissions.Any(y =>
        //                y.LastSubmittedBy.Equals("test", StringComparison.CurrentCultureIgnoreCase) &&
        //                y.LastSubmittedDateUtc == new DateTime(2018, 10, 12) &&
        //                y.CollectionName == "ILR1819"))
        //        .Should()
        //        .BeTrue();
        // }

        [Fact]
        public async Task Add_Duplicate_Should_ReturnOk()
        {
            var orgnisationServiceMock = new Mock<IOrganisationService>();
            var jobqueryServiceMock = new Mock<IJobQueryService>();
            var loggerMock = new Mock<ILogger>();
            var auditFactoryMock = new Mock<IAuditFactory>();

            orgnisationServiceMock.Setup(x => x.GetByUkprn(It.IsAny<long>()))
                .ReturnsAsync((Organisation)null);
            orgnisationServiceMock.Setup(x => x.AddOrganisation(It.IsAny<Organisation>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var orgController = new OrganisationController(orgnisationServiceMock.Object, null, jobqueryServiceMock.Object, loggerMock.Object, auditFactoryMock.Object, null);
            var result = await orgController.AddOrganisation(new Organisation(), CancellationToken.None);
            result.Should().NotBeNull();
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        private List<Provider> ProvidersList()
        {
            var providers = new List<Provider>
            {
                new Provider
                {
                    Name = "Org1",
                    TradingName = "OrgTrade1",
                    Ukprn = 10000001,
                    Upin = "100001",
                },
                new Provider
                {
                    Name = "Org2",
                    TradingName = "OrgTrade2",
                    Ukprn = 10000002,
                    Upin = "100002",
                },
                new Provider
                {
                    Name = "Org3",
                    TradingName = "OrgTrade3",
                    Ukprn = 10000003,
                    Upin = "100003",
                },
                new Provider
                {
                    Name = "Org4",
                    TradingName = "OrgTrade4",
                    Ukprn = 10000004,
                    Upin = "100004",
                },
            };

            return providers;
        }
    }
}
