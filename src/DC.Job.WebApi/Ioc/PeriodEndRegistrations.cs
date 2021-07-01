using Autofac;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Job.WebApi.Extensions;
using ESFA.DC.Job.WebApi.Settings;
using ESFA.DC.PeriodEnd.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Service.Clients;
using ESFA.DC.PeriodEnd.Services;
using ESFA.DC.PeriodEnd.Services.ALLF.Controllers;
using ESFA.DC.PeriodEnd.Services.ALLF.Strategies;
using ESFA.DC.PeriodEnd.Services.Comparers;
using ESFA.DC.PeriodEnd.Services.Email;
using ESFA.DC.PeriodEnd.Services.Factories;
using ESFA.DC.PeriodEnd.Services.ILR.Controllers;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.CollectionStatsPath;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.DASStartedPath;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.DataWarehouse1Path;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.DataWarehouse2Path;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart1Path;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart2Path;
using ESFA.DC.PeriodEnd.Services.ILR.Strategies.InternalReportsPath;
using ESFA.DC.PeriodEnd.Services.NCS.Controllers;
using ESFA.DC.PeriodEnd.Services.NCS.Strategies;
using ESFA.DC.PeriodEnd.Services.Persistence;

namespace ESFA.DC.Job.WebApi.Ioc
{
    public class PeriodEndRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var config = c.Resolve<CloudStorageSettings>();
                    return new AzureStorageFileServiceConfiguration
                    {
                        ConnectionString = config.ConnectionStrings["PeriodEndConnectionString"]
                    };
                })
                .As<IAzureStorageFileServiceConfiguration>().SingleInstance();

            builder.RegisterType<AzureStorageFileService>().As<IFileService>();

            builder.RegisterType<PeriodEndServiceALLF>().As<IPeriodEndServiceALLF>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndServiceILR>().As<IPeriodEndServiceILR>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndServiceNCS>().As<IPeriodEndServiceNCS>().InstancePerLifetimeScope();

            builder.RegisterType<FailedJobQueryServiceILR>().As<IFailedJobQueryServiceILR>().InstancePerLifetimeScope();
            builder.RegisterType<FailedJobQueryServiceNCS>().As<IFailedJobQueryServiceNCS>().InstancePerLifetimeScope();

            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();
            builder.RegisterType<PathStructureServiceILR>().As<IPathStructureServiceILR>().InstancePerLifetimeScope();
            builder.RegisterType<PathStructureServiceNCS>().As<IPathStructureServiceNCS>().InstancePerLifetimeScope();
            builder.RegisterType<PathStructureServiceALLF>().As<IPathStructureServiceALLF>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndOutputService>().As<IPeriodEndOutputService>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndDateTimeService>().As<IPeriodEndDateTimeService>().InstancePerLifetimeScope();

            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndEmailService>().As<IPeriodEndEmailService>().InstancePerLifetimeScope();

            builder.RegisterType<CommsRepository>().As<ICommsRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndRepository>().As<IPeriodEndRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FileUploadRepository>().As<IFileUploadRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QueryService>().As<IQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailDistributionRepository>().As<IEmailDistributionRepository>().ExternallyOwned();

            builder.RegisterType<PathComparer>().As<IPathComparer>().InstancePerLifetimeScope();
            builder.RegisterType<SubPathItemComparer>().As<ISubPathItemComparer>().InstancePerLifetimeScope();
            builder.RegisterType<ValidityPeriodRepository>().As<IValidityPeriodRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ValidityStructureService>().As<IValidityStructureService>().InstancePerLifetimeScope();

            builder.RegisterType<CriticalPathController>().As<ICriticalPathController>();
            builder.RegisterType<DASStartedController>().As<IDASStartedController>();
            builder.RegisterType<DataWarehouse1Controller>().As<IDataWarehouse1Controller>();
            builder.RegisterType<DataWarehouse2Controller>().As<IDataWarehouse2Controller>();
            builder.RegisterType<InternalReportsController>().As<IInternalReportsController>();
            builder.RegisterType<FCSHandOverPart1Controller>().As<IFCSHandOverPart1Controller>();
            builder.RegisterType<FCSHandOverPart2Controller>().As<IFCSHandOverPart2Controller>();

            builder.RegisterType<CriticalPathController>().As<IPathController>();
            builder.RegisterType<DASStartedController>().As<IPathController>();
            builder.RegisterType<DataWarehouse1Controller>().As<IPathController>();
            builder.RegisterType<DataWarehouse2Controller>().As<IPathController>();
            builder.RegisterType<InternalReportsController>().As<IPathController>();
            builder.RegisterType<FCSHandOverPart1Controller>().As<IPathController>();
            builder.RegisterType<FCSHandOverPart2Controller>().As<IPathController>();

            builder.RegisterType<NCSPathController>().As<IPathControllerNCS>();

            builder.RegisterType<ALLFPathController>().As<IPathControllerALLF>();

            builder.RegisterType<PeriodEndJobFactory>().As<IPeriodEndJobFactory>();
            builder.RegisterType<PathItemParamsFactory>().As<IPathItemParamsFactory>();
            builder.RegisterType<PathItemReturnFactory>().As<IPathItemReturnFactory>();

            builder.RegisterType<ValidityPeriodService>().As<IValidityPeriodService>();

            builder.RegisterType<ReportFileServiceILR>().As<IReportFileServiceILR>();
            builder.RegisterType<ReportFileServiceNCS>().As<IReportFileServiceNCS>();
            builder.RegisterType<EmailDistributionService>().As<IEmailDistributionService>();
            builder.RegisterType<HistoryILRService>().As<IHistoryILRService>();
            builder.RegisterType<HistoryNCSService>().As<IHistoryNCSService>();
            builder.RegisterType<CollectionStatsService>().As<ICollectionStatsService>();

            builder.RegisterType<DASPaymentsService>().As<IDASPaymentsService>();
            builder.RegisterType<DASClientService>().As<IDASClientService>();

            builder.RegisterType<DASStopping>().As<IDASStoppingPathItem>();

            builder.RegisterType<DataQualityReport>().As<IPathItem>();
            builder.RegisterType<ProviderSubmissionsReport>().As<IPathItem>();
            builder.RegisterType<EsfaNonContractedDevolvedAdultEducationActivityReport>().As<IPathItem>();
            builder.RegisterType<DataExtractReport>().As<IPathItem>();

            builder.RegisterType<InternalDataMatchReport>().As<IPathItem>();
            builder.RegisterType<ActCountReport>().As<IPathItem>();

            builder.RegisterType<ReturnCodeService>().As<IReturnCodeService>();

            builder.RegisterType<DataWarehouse1>().As<IILRPathItem>();

            builder.RegisterType<DataWarehouse2>().As<IILRPathItem>();

            builder.RegisterType<StandardFile>().As<IILRPathItem>();

            builder.RegisterType<DataQualityReport>().As<IILRPathItem>();
            builder.RegisterType<ProviderSubmissionsReport>().As<IILRPathItem>();
            builder.RegisterType<CollectionStatsPathItem>().As<IILRPathItem>();
            builder.RegisterType<EsfaNonContractedDevolvedAdultEducationActivityReport>().As<IILRPathItem>();

            builder.RegisterOrdered<IILRPathItem>(scope =>
            {
                scope.Register<HiddenPeriodEndStart>();

                scope.Register<ReferenceDataFCS>();

                scope.Register<DCSummarisation>();
                scope.Register<ESFSummarisation>();

                scope.Register<HiddenFCSPart1HandOver>();

                scope.Register<McaReport>();

                scope.Register<NonSubmittingProviders>();
                scope.Register<DASReprocessing>();

                scope.Register<DASStarting>();
                scope.Register<DASSubmissionWindowPeriodValidation>();

                scope.Register<HiddenDataWarehouse1>();

                scope.Register<DASRunning>();

                scope.Register<DASMetrics>();

                scope.Register<AppSummarisation>();
                scope.Register<HiddenFCSPart2HandOver>();

                scope.Register<DataExtractReport>();
                scope.Register<DataExtractReportEmail>();

                scope.Register<InternalDataMatchReport>();
                scope.Register<ActCountReport>();

                scope.Register<DASPeriodEndReportPreparation>();
                scope.Register<AppsAdditionalPaymentsReport>();
                scope.Register<AppsCoInvestmentContributionsReport>();
                scope.Register<AppsMonthlyPaymentReport>();
                scope.Register<FundingSummaryPeriodEndReport>();
                scope.Register<AppsDataMatchMonthEndReport>();
                scope.Register<LLVReport>();

                scope.Register<CrossYearIndicativePaymentsReport>();

                scope.Register<ReportsAvailableEmail>();

                scope.Register<HiddenDataWarehouse2>();
            });

            builder.RegisterOrdered<IDataWarehouse1PathItem>(scope =>
            {
                scope.Register<DataWarehouse1>();
                scope.Register<DataWarehouse1Email>();
            });

            builder.RegisterOrdered<IDataWarehouse2PathItem>(scope =>
            {
                scope.Register<DataWarehouse2>();
                scope.Register<DataWarehouse2Email>();
            });

            builder.RegisterOrdered<IDASStartedPathItem>(scope =>
            {
                scope.Register<DASStartedEmail>();
            });

            builder.RegisterOrdered<IFCSHandOverPart1PathItem>(scope =>
            {
                scope.Register<FCSPart1InitialBlockingItem>();
                scope.Register<FCSTeamHandOverPart1Email>();
            });

            builder.RegisterOrdered<IFCSHandOverPart2PathItem>(scope =>
            {
                scope.Register<FCSPart2InitialBlockingItem>();
                scope.Register<FCSBroadcastHandOverPart2Email>();
                scope.Register<FCSTeamHandOverPart2Email>();

                scope.Register<StandardFileInitialBlockingItem>();
                scope.Register<StandardFileEmail>();
                scope.Register<StandardFile>();
            });

            builder.RegisterOrdered<IInternalReportsPathItem>(scope =>
            {
                scope.Register<DataQualityReport>();
                scope.Register<DataQualityReportEmail>();
                scope.Register<ProviderSubmissionsReport>();
                scope.Register<ProviderSubmissionsReportEmail>();
                scope.Register<CollectionStatsPathItem>();
                scope.Register<EsfaNonContractedDevolvedAdultEducationActivityReport>();
                scope.Register<EsfaNonContractedDevolvedAdultEducationActivityReportEmail>();
            });

            builder.RegisterOrdered<INCSPathItem>(scope =>
            {
                scope.Register<NCSSummarisation>();
                scope.Register<NCSFCSHandOverEmail>();
                scope.Register<NCSDataExtractReport>();
            });

            builder.RegisterOrdered<IALLFPathItem>(scope =>
            {
                scope.Register<ALLFSummarisation>();
                scope.Register<ALLFFCSHandOverEmail>();
            });
        }
    }
}