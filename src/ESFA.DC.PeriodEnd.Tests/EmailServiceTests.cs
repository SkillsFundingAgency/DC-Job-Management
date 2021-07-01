using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Services;
using Moq;
using Xunit;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class EmailServiceTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Mock<ICommsRepository> _mockCommsRepository;
        private readonly Mock<IEmailNotifier> _mockEmailNotifier;
        private readonly Mock<IAuditFactory> _mockAuditFactory;
        private readonly EmailService _sut;

        public EmailServiceTests()
        {
            _logger = new Mock<ILogger>();
            _mockCommsRepository = new Mock<ICommsRepository>();
            _mockEmailNotifier = new Mock<IEmailNotifier>();
            _mockAuditFactory = new Mock<IAuditFactory>();
            _mockAuditFactory.Setup(x => x.BuildRequestAudit<SendEmailDTO>(It.IsAny<Func<SendEmailDTO>>()))
                .Returns(new Mock<IAudit>().Object);

            _sut = new EmailService(_logger.Object, _mockCommsRepository.Object, _mockEmailNotifier.Object, _mockAuditFactory.Object);
        }

        [Fact]
        public async Task SendEmail_WithNoRecipients_RaisesError()
        {
            // Arrange
            _mockCommsRepository.Setup(c => c.GetEmailDetails(0)).ReturnsAsync(new Models.PathItemEmailDetails());

            // Act
            Task result = _sut.SendEmail(0, null);

            // Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () => await result);
            Assert.Equal("No email details found for emailId 0", ex.Message);
        }

        [Fact]
        public async Task SendEmail_SendsEmailToEachRecipient()
        {
            // Arrange
            var emailDetails = new Models.PathItemEmailDetails
            {
                Recipients = new List<string> { "One", "Two", "Three" },
                TemplateId = "TemplateId"
            };

            _mockCommsRepository.Setup(c => c.GetEmailDetails(0)).ReturnsAsync(emailDetails);

            // Act
            await _sut.SendEmail(0, null);

            // Assert
            _mockEmailNotifier.Verify(e => e.SendEmail(emailDetails.Recipients.ElementAt(0), It.IsAny<string>(), null), Times.Once());
            _mockEmailNotifier.Verify(e => e.SendEmail(emailDetails.Recipients.ElementAt(1), It.IsAny<string>(), null), Times.Once());
            _mockEmailNotifier.Verify(e => e.SendEmail(emailDetails.Recipients.ElementAt(2), It.IsAny<string>(), null), Times.Once());
        }
    }
}
