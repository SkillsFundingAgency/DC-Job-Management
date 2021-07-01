using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Interface;
using ESFA.DC.DashBoard.Models;
using ESFA.DC.DashBoard.Models.Job;
using ESFA.DC.DashBoard.Models.ServiceBus;

namespace ESFA.DC.DashBoard.Services
{
    public sealed class DashBoardService : IDashBoardService
    {
        private readonly IServiceBusStatsService _serviceBusStatsService;
        private readonly IJobService _jobService;

        public DashBoardService(
            IServiceBusStatsService serviceBusStatsService,
            IJobService jobService)
        {
            _serviceBusStatsService = serviceBusStatsService;
            _jobService = jobService;
        }

        public async Task<DashBoardModel> ProvideAsync(CancellationToken cancellationToken)
        {
            Task<JobStatsModel> jobStats = _jobService.ProvideAsync(cancellationToken);
            Task<ServiceBusStatusModel> serviceBusStats = _serviceBusStatsService.ProvideAsync(cancellationToken);

            await Task.WhenAll(serviceBusStats, jobStats);

            DashBoardModel dashBoardModel = new DashBoardModel
            {
                ServiceBusStats = serviceBusStats.Result,
                JobStats = jobStats.Result
            };

            return dashBoardModel;
        }
    }
}