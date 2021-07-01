using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class ServiceMessageTests
    {
        [Fact]
        public async Task GetMessage_Null()
        {
            IContainer container = Registrations();

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();
                }

                IServiceMessageService service = scope.Resolve<IServiceMessageService>();
                service.GetMessageAsync(It.IsAny<DateTime>(), string.Empty, It.IsAny<CancellationToken>()).Result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetMessage_NotEnabled()
        {
            IContainer container = Registrations();

            var dateTimeUtc = DateTime.UtcNow;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();

                    context.ServiceMessage.Add(new ServiceMessage()
                    {
                        StartDateTimeUtc = dateTimeUtc.AddDays(-1),
                        Enabled = false,
                        Message = "test"
                    });
                    context.SaveChanges();
                }

                IServiceMessageService service = scope.Resolve<IServiceMessageService>();
                service.GetMessageAsync(dateTimeUtc, string.Empty, CancellationToken.None).Result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetMessage_OutsidedateRange()
        {
            IContainer container = Registrations();

            var dateTimeUtc = DateTime.UtcNow;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();

                    context.ServiceMessage.Add(new ServiceMessage()
                    {
                        StartDateTimeUtc = dateTimeUtc.AddDays(1),
                        Enabled = false,
                        Message = "test"
                    });
                    context.SaveChanges();
                }

                IServiceMessageService service = scope.Resolve<IServiceMessageService>();
                service.GetMessageAsync(dateTimeUtc, string.Empty, CancellationToken.None).Result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetMessage_ValidMessage()
        {
            IContainer container = Registrations();

            var controllerName = "foo";
            var dateTimeUtc = DateTime.UtcNow;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();

                    var page = context.ServicePage.Add(new ServicePage
                    {
                        DisplayName = "test",
                        ControllerName = controllerName
                    });

                    var message = context.ServiceMessage.Add(new ServiceMessage
                    {
                        StartDateTimeUtc = dateTimeUtc,
                        Enabled = true,
                        Message = "Test message"
                    });

                    context.SaveChanges();

                    context.ServicePageMessage.Add(new ServicePageMessage
                    {
                        PageId = page.Entity.Id,
                        MessageId = message.Entity.Id
                    });

                    context.SaveChanges();
                }

                IServiceMessageService service = scope.Resolve<IServiceMessageService>();
                var result = service.GetMessageAsync(dateTimeUtc, controllerName, CancellationToken.None).Result;

                result.Should().Be("Test message");
            }
        }

        [Fact]
        public async Task GetMessage_ValidMessage_BetweenDates()
        {
            IContainer container = Registrations();

            var controllerName = "foo";
            var dateTimeUtc = DateTime.UtcNow;

            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();

                    var page = context.ServicePage.Add(new ServicePage
                    {
                        DisplayName = "test",
                        ControllerName = controllerName
                    });

                    var message = context.ServiceMessage.Add(new ServiceMessage
                    {
                        StartDateTimeUtc = dateTimeUtc,
                        Enabled = true,
                        Message = "Test message",
                        EndDateTimeUtc = dateTimeUtc.AddMinutes(1)
                    });
                    context.SaveChanges();

                    context.ServicePageMessage.Add(new ServicePageMessage
                    {
                        PageId = page.Entity.Id,
                        MessageId = message.Entity.Id
                    });

                    context.SaveChanges();
                }

                IServiceMessageService service = scope.Resolve<IServiceMessageService>();
                var result = service.GetMessageAsync(dateTimeUtc, controllerName, CancellationToken.None).Result;

                result.Should().Be("Test message");
            }
        }

        private IContainer Registrations()
        {
            ContainerBuilder builder = new ContainerBuilder();

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

            builder.RegisterInstance(dateTimeProviderMock.Object).As<IDateTimeProvider>().SingleInstance();
            builder.RegisterInstance(new Mock<IEmailNotifier>().Object).As<IEmailNotifier>().SingleInstance();
            builder.RegisterInstance(new Mock<INcsJobManager>().Object).As<INcsJobManager>().SingleInstance();
            builder.RegisterInstance(new Mock<IJobEmailTemplateManager>().Object).As<IJobEmailTemplateManager>().SingleInstance();
            builder.RegisterInstance(new Mock<ILogger>().Object).As<ILogger>().SingleInstance();
            builder.RegisterInstance(new Mock<IReturnCalendarService>().Object).As<IReturnCalendarService>().SingleInstance();
            builder.RegisterInstance(new Mock<IAuditFactory>().Object).As<IAuditFactory>().SingleInstance();
            builder.RegisterType<NcsJobManager>().As<INcsJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().InstancePerDependency();
            builder.RegisterType<ServiceMessageService>().As<IServiceMessageService>().InstancePerDependency();

            builder.Register(context =>
                {
                    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    DbContextOptionsBuilder<JobQueueDataContext> optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>()
                        .UseSqlite(connection);
                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            IContainer container = builder.Build();
            return container;
        }
    }
}
