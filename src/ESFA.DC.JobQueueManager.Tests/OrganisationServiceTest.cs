using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using ESFA.DC.Audit.Models.DTOs.Providers;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class OrganisationServiceTest
    {
        private readonly AutoMock _mocker;
        private readonly OrganisationService _sut;

        public OrganisationServiceTest()
        {
            _mocker = AutoMock.GetLoose();
            _sut = _mocker.Create<OrganisationService>();
        }

        [Fact]
        public async Task AddBulkOrganisationCollectionsAsync_ShouldChecksCollectionNames()
        {
            var data = new List<OrganisationCollection>
            {
                new OrganisationCollection { }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddBulkOrganisationCollectionsAsync(data));
            ex.Message.Should().Be("Missing Collection Name");
        }

        [Fact]
        public async Task AddBulkOrganisationCollectionsAsync_ShouldChecksStartEndDates()
        {
            var data = new List<OrganisationCollection>
            {
                new OrganisationCollection { CollectionName = "Test", StartDate = DateTime.Now, EndDate = DateTime.Now }
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddBulkOrganisationCollectionsAsync(data));
            ex.Message.Should().Be("Start and End dates both specified");
        }

        [Fact]
        public async Task AddBulkOrganisationCollectionsAsync_ShouldChecksMissingOrganisations()
        {
            var data = new List<OrganisationCollection>
            {
                new OrganisationCollection { Ukprn = 50, CollectionName = "Test", StartDate = DateTime.Now }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var databaseOrganisation = new List<Data.Entities.Organisation>().AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _sut.AddBulkOrganisationCollectionsAsync(data));
            ex.Message.Should().Be("Missing Organisations");
        }

        [Fact]
        public async Task AddBulkOrganisationCollectionsAsync_ShouldAddOrganisationCollection()
        {
            // Arrange
            var data = new List<OrganisationCollection>
            {
                new OrganisationCollection { Ukprn = 50, CollectionName = "Test", StartDate = DateTime.Parse("2021-01-01T01:00") }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var databaseOrganisation = new List<Data.Entities.Organisation>
            {
                new Data.Entities.Organisation
                {
                    Ukprn = 50L
                }
            }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);
            var databaseOrganisationCollection = new List<Data.Entities.OrganisationCollection>().AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.OrganisationCollection).Returns(databaseOrganisationCollection.Object);
            var databaseCollection = new List<Data.Entities.Collection> { new Data.Entities.Collection { Name = "Test", CollectionId = 10, CollectionType = new Data.Entities.CollectionType { IsProviderAssignableInOperations = true } } }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Collection).Returns(databaseCollection.Object);
            _mocker.Mock<IDateTimeProvider>().Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(dt => dt.AddHours(-1));

            // Act
            await _sut.AddBulkOrganisationCollectionsAsync(data);

            // Assert
            Func<object, bool> verifyAdd = o =>
            {
                var setEndDate = JsonConvert.DeserializeObject<List<Data.Entities.OrganisationCollection>>(o.GetType().GetProperty("setEndDate").GetValue(o).ToString());
                var removeEndDate = JsonConvert.DeserializeObject<List<Data.Entities.OrganisationCollection>>(o.GetType().GetProperty("removeEndDate").GetValue(o).ToString());
                var add = JsonConvert.DeserializeObject<List<Data.Entities.OrganisationCollection>>(o.GetType().GetProperty("add").GetValue(o).ToString());

                add.Should().HaveCount(1);
                setEndDate.Should().BeEmpty();
                removeEndDate.Should().BeEmpty();

                add.First().StartDateTimeUtc.Should().Be(DateTime.Parse("2021-01-01"));

                return true;
            };
            mockContext.Verify(x => x.FromSqlAsync<int>(CommandType.StoredProcedure, "BulkAddProviderCollections", It.Is<object>(o => verifyAdd(o))), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignments_ShouldReturnFalseIfNoCollectionFound()
        {
            // Arrange
            var ct = CancellationToken.None;
            var ukprn = 100L;
            var organisationCollections = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    CollectionId = 50,
                }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var databaseCollection = new List<Data.Entities.Collection>().AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Collection).Returns(databaseCollection.Object);
            var databaseOrganisation = new List<Data.Entities.Organisation>
            {
                new Data.Entities.Organisation
                {
                    Ukprn = ukprn
                }
            }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);

            _mocker.Mock<IAuditFactory>().Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<EditProviderAssignmentsDTO>>>(), mockContext.Object)).Returns(_mocker.Mock<IAudit>().Object);

            // Act
            var result = await _sut.UpdateAssignments(ukprn, organisationCollections, ct);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAssignments_ShouldReturnFalseIfNoOrganisationFound()
        {
            // Arrange
            var ct = CancellationToken.None;
            var ukprn = 100L;
            var organisationCollections = new List<OrganisationCollection>
            {
                new OrganisationCollection()
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var databaseOrganisation = new List<Data.Entities.Organisation>().AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);

            // Act
            var result = await _sut.UpdateAssignments(ukprn, organisationCollections, ct);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAssignments_ShouldAddOrganisationCollection()
        {
            // Arrange
            var ct = CancellationToken.None;
            var ukprn = 100L;
            var organisationCollections = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    CollectionId = 50,
                    StartDate = DateTime.Parse("2021-01-01T01:00"),
                    EndDate = DateTime.Parse("2021-02-02T01:00")
                }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var collection = new Data.Entities.Collection
            {
                CollectionId = 50
            };
            var databaseCollection = new List<Data.Entities.Collection> { collection }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Collection).Returns(databaseCollection.Object);
            var organisation = new Data.Entities.Organisation
            {
                Ukprn = ukprn
            };
            var databaseOrganisation = new List<Data.Entities.Organisation> { organisation }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);
            var resultOrganisationCollections = new List<Data.Entities.OrganisationCollection>();
            var databaseOrganisationCollection = resultOrganisationCollections.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.OrganisationCollection).Returns(databaseOrganisationCollection.Object);

            _mocker.Mock<IAuditFactory>().Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<EditProviderAssignmentsDTO>>>(), mockContext.Object)).Returns(_mocker.Mock<IAudit>().Object);
            _mocker.Mock<IDateTimeProvider>().Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(dt => dt.AddHours(-1));

            // Act
            var result = await _sut.UpdateAssignments(ukprn, organisationCollections, ct);

            // Assert
            result.Should().BeTrue();
            databaseOrganisationCollection.Verify(
                x => x.AddAsync(
                    It.Is<Data.Entities.OrganisationCollection>(
                oc => oc.StartDateTimeUtc == DateTime.Parse("2021-01-01") && oc.Ukprn == ukprn && oc.Organisation == organisation && oc.Collection == collection && oc.CollectionId == 50 && oc.EndDateTimeUtc == DateTime.Parse("2021-02-02")), ct), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignments_ShouldAddOrganisationCollection_NullEndDate()
        {
            // Arrange
            var ct = CancellationToken.None;
            var ukprn = 100L;
            var organisationCollections = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    CollectionId = 50,
                    StartDate = DateTime.Parse("2021-01-01T01:00"),
                }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var collection = new Data.Entities.Collection
            {
                CollectionId = 50
            };
            var databaseCollection = new List<Data.Entities.Collection> { collection }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Collection).Returns(databaseCollection.Object);
            var organisation = new Data.Entities.Organisation
            {
                Ukprn = ukprn
            };
            var databaseOrganisation = new List<Data.Entities.Organisation> { organisation }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);
            var resultOrganisationCollections = new List<Data.Entities.OrganisationCollection>();
            var databaseOrganisationCollection = resultOrganisationCollections.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.OrganisationCollection).Returns(databaseOrganisationCollection.Object);

            _mocker.Mock<IAuditFactory>().Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<EditProviderAssignmentsDTO>>>(), mockContext.Object)).Returns(_mocker.Mock<IAudit>().Object);
            _mocker.Mock<IDateTimeProvider>().Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(dt => dt.AddHours(-1));

            // Act
            var result = await _sut.UpdateAssignments(ukprn, organisationCollections, ct);

            // Assert
            result.Should().BeTrue();
            databaseOrganisationCollection.Verify(
                x => x.AddAsync(
                    It.Is<Data.Entities.OrganisationCollection>(
                oc => oc.EndDateTimeUtc == null), ct), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignments_ShouldUpdateOrganisationCollection()
        {
            // Arrange
            var ct = CancellationToken.None;
            var ukprn = 100L;
            var organisationCollections = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    CollectionId = 50,
                    StartDate = DateTime.Parse("2021-01-01T01:00"),
                    EndDate = DateTime.Parse("2021-02-02T01:00")
                }
            };

            var mockContext = _mocker.Mock<IJobQueueDataContext>();
            var collection = new Data.Entities.Collection
            {
                CollectionId = 50
            };
            var databaseCollection = new List<Data.Entities.Collection> { collection }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Collection).Returns(databaseCollection.Object);
            var organisation = new Data.Entities.Organisation
            {
                Ukprn = ukprn
            };
            var databaseOrganisation = new List<Data.Entities.Organisation> { organisation }.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.Organisation).Returns(databaseOrganisation.Object);
            var organisationCollection = new Data.Entities.OrganisationCollection
            {
                Organisation = organisation,
                Collection = collection
            };
            var resultOrganisationCollections = new List<Data.Entities.OrganisationCollection> { organisationCollection };
            var databaseOrganisationCollection = resultOrganisationCollections.AsQueryable().BuildMockDbSet();
            mockContext.Setup(x => x.OrganisationCollection).Returns(databaseOrganisationCollection.Object);

            _mocker.Mock<IAuditFactory>().Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<EditProviderAssignmentsDTO>>>(), mockContext.Object)).Returns(_mocker.Mock<IAudit>().Object);
            _mocker.Mock<IDateTimeProvider>().Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(dt => dt.AddHours(-1));

            // Act
            var result = await _sut.UpdateAssignments(ukprn, organisationCollections, ct);

            // Assert
            result.Should().BeTrue();
            organisationCollection.StartDateTimeUtc.Should().Be(DateTime.Parse("2021-01-01"));
            organisationCollection.EndDateTimeUtc.Should().Be(DateTime.Parse("2021-02-02"));
            mockContext.Verify(x => x.SaveChangesAsync(ct), Times.Once);
        }
    }

    internal class BulkAddProviderCollections
    {
        public string add { get; set; }

        public string removeEndDate { get; set; }

        public string setEndDate { get; set; }
    }
}