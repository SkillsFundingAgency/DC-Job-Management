using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Settings;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;
using ReturnPeriod = ESFA.DC.CollectionsManagement.Models.ReturnPeriod;

namespace ESFA.DC.JobQueueManager.Tests
{
    public class EmailTemplateManagerTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetTemplate_Success(bool isClose)
        {
            var returnCalendarMock = new Mock<IReturnCalendarService>();
            returnCalendarMock.Setup(x => x.GetPeriodAsync(1, It.IsAny<DateTime>())).ReturnsAsync(() => isClose ? null : new ReturnPeriod());

            IContainer container = Registrations(returnCalendarMock.Object);

            using (var scope = container.BeginLifetimeScope())
            {
                var templateManager = scope.Resolve<IJobEmailTemplateManager>();
                var options = scope.Resolve<DbContextOptions<JobQueueDataContext>>();

                // Create the schema in the database
                using (var context = new JobQueueDataContext(options))
                {
                    context.Database.EnsureCreated();
                    context.JobEmailTemplate.Add(new JobEmailTemplate()
                    {
                        CollectionId = 1,
                        JobStatus = (short)JobStatusType.Completed,
                        Active = true,
                        TemplateOpenPeriod = "template_open",
                        TemplateClosePeriod = "template_close",
                    });

                    if (!isClose)
                    {
                        context.ReturnPeriod.Add(new Data.Entities.ReturnPeriod()
                        {
                            PeriodNumber = 1,
                            CollectionId = 1,
                        });
                    }

                    context.FileUploadJobMetaData.Add(new FileUploadJobMetaData()
                    {
                        Job = new Job()
                        {
                            JobId = 1,
                            Collection = new Collection()
                            {
                                CollectionId = 1,
                                Name = CollectionConstants.ILR2021,
                            },
                            Status = (short)JobStatusType.Completed,
                        },

                        PeriodNumber = 1,
                        FileName = "test",
                        FileSize = 100,
                        StorageReference = "test",
                    });
                    context.SaveChanges();
                }

                var template =
                    templateManager.GetTemplate(1, DateTime.Now).Result;
                template.Should().NotBeNull();
                template.Should().Be(isClose ? "template_close" : "template_open");
            }
        }

        private DbContextOptions<JobQueueDataContext> GetContextOptions([CallerMemberName] string functionName = "")
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<JobQueueDataContext>()
                .UseInMemoryDatabase(functionName)
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            return options;
        }

        private IContainer Registrations(IReturnCalendarService returnCalendarService = null)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterInstance(new Mock<IDateTimeProvider>().Object).As<IDateTimeProvider>().SingleInstance();
            builder.RegisterInstance(returnCalendarService ?? new Mock<IReturnCalendarService>().Object).As<IReturnCalendarService>().SingleInstance();
            builder.RegisterType<JobEmailTemplateManager>().As<IJobEmailTemplateManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueryService>().As<IJobQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailNotifier>().As<IEmailNotifier>().ExternallyOwned();
            builder.RegisterInstance(new Mock<ILogger>().Object).As<ILogger>().SingleInstance();
            builder.RegisterInstance(new Mock<INotifierConfig>().Object).As<INotifierConfig>().SingleInstance();

            builder.Register(c =>
            {
                return new JobQueueManagerSettings
                {
                    ConnectionString = string.Empty
                };
            }).As<JobQueueManagerSettings>().SingleInstance();

            builder.Register(context =>
                {
                    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    return GetContextOptions();
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionString = "Server=(local);Database=JobManagement;Integrated Security=True;";
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();

            builder.Register(c =>
            {
                var options = c.Resolve<DbContextOptions<OrganisationsContext>>();

                return new OrganisationsContext(options);
            }).As<IOrganisationsContext>().ExternallyOwned();

            IContainer container = builder.Build();
            return container;
        }
    }
}
