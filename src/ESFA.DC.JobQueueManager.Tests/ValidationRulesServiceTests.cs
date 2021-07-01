using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.ILR2122.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class ValidationRulesServiceTests
    {
        [Fact]
        public async Task GetILR1819ValidationRulesAsync()
        {
            var cancellationToken = CancellationToken.None;

            var ilrValidationErrors = new List<ILR1819.DataStore.EF.ValidationError>
            {
                new ILR1819.DataStore.EF.ValidationError { RuleName = "Rule1" },
                new ILR1819.DataStore.EF.ValidationError { RuleName = "Rule2" },
            }.AsQueryable().BuildMockDbSet();

            var ilrContextMock = new Mock<IIlr1819RulebaseContext>();
            ilrContextMock.Setup(x => x.ValidationErrors).Returns(ilrValidationErrors.Object);

            var ilrFuncMock = new Mock<Func<IIlr1819RulebaseContext>>();
            ilrFuncMock.Setup(x => x.Invoke()).Returns(ilrContextMock.Object);

            var valMessagesFunc = BuildValidationMessagesFuncForYear(1819);

            var service = new ValidationRules1819Service(ilrFuncMock.Object, valMessagesFunc);

            var rules = await service.GetILRValidationRulesAsync(cancellationToken);

            rules.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetILR1920ValidationRulesAsync()
        {
            var cancellationToken = CancellationToken.None;

            var ilrValidationErrors = new List<ILR1920.DataStore.EF.ValidationError>
            {
                new ILR1920.DataStore.EF.ValidationError { RuleName = "Rule1" },
                new ILR1920.DataStore.EF.ValidationError { RuleName = "Rule2" },
            }.AsQueryable().BuildMockDbSet();

            var ilrContextMock = new Mock<IIlr1920RulebaseContext>();
            ilrContextMock.Setup(x => x.ValidationErrors).Returns(ilrValidationErrors.Object);

            var ilrFuncMock = new Mock<Func<IIlr1920RulebaseContext>>();
            ilrFuncMock.Setup(x => x.Invoke()).Returns(ilrContextMock.Object);

            var valMessagesFunc = BuildValidationMessagesFuncForYear(1920);

            var service = new ValidationRules1920Service(ilrFuncMock.Object, valMessagesFunc);

            var rules = await service.GetILRValidationRulesAsync(cancellationToken);

            rules.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetILR2021ValidationRulesAsync()
        {
            var cancellationToken = CancellationToken.None;

            var ilrValidationErrors = new List<ILR2021.DataStore.EF.ValidationError>
            {
                new ILR2021.DataStore.EF.ValidationError { RuleName = "Rule1" },
                new ILR2021.DataStore.EF.ValidationError { RuleName = "Rule2" },
            }.AsQueryable().BuildMockDbSet();

            var ilrContextMock = new Mock<IIlr2021RulebaseContext>();
            ilrContextMock.Setup(x => x.ValidationErrors).Returns(ilrValidationErrors.Object);

            var ilrFuncMock = new Mock<Func<IIlr2021RulebaseContext>>();
            ilrFuncMock.Setup(x => x.Invoke()).Returns(ilrContextMock.Object);

            var valMessagesFunc = BuildValidationMessagesFuncForYear(2021);

            var service = new ValidationRules2021Service(ilrFuncMock.Object, valMessagesFunc);

            var rules = await service.GetILRValidationRulesAsync(cancellationToken);

            rules.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetILR2122ValidationRulesAsync()
        {
            var cancellationToken = CancellationToken.None;

            var ilrValidationErrors = new List<ILR2122.DataStore.EF.ValidationError>
            {
                new ILR2122.DataStore.EF.ValidationError { RuleName = "Rule1" },
                new ILR2122.DataStore.EF.ValidationError { RuleName = "Rule2" },
            }.AsQueryable().BuildMockDbSet();

            var ilrContextMock = new Mock<IIlr2122Context>();
            ilrContextMock.Setup(x => x.ValidationErrors).Returns(ilrValidationErrors.Object);

            var ilrFuncMock = new Mock<Func<IIlr2122Context>>();
            ilrFuncMock.Setup(x => x.Invoke()).Returns(ilrContextMock.Object);

            var valMessagesFunc = BuildValidationMessagesFuncForYear(2122);

            var service = new ValidationRules2122Service(ilrFuncMock.Object, valMessagesFunc);

            var rules = await service.GetILRValidationRulesAsync(cancellationToken);

            rules.Should().HaveCount(2);
        }

        private Func<IValidationMessagesContext> BuildValidationMessagesFuncForYear(int year)
        {
            var validationMessages = new List<Message>
            {
                new Message { CollectionYear = year, RuleName = "Rule1" },
                new Message { CollectionYear = year, RuleName = "Rule2" },
            }.AsQueryable().BuildMockDbSet();

            var valMessagesContextMock = new Mock<IValidationMessagesContext>();
            valMessagesContextMock.Setup(x => x.Messages).Returns(validationMessages.Object);

            var valMessagesFuncMock = new Mock<Func<IValidationMessagesContext>>();
            valMessagesFuncMock.Setup(x => x.Invoke()).Returns(valMessagesContextMock.Object);

            return valMessagesFuncMock.Object;
        }
    }
}
