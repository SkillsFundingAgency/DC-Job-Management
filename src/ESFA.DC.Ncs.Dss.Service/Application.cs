using System.Threading;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Ncs.Dss.Service.Dtos;
using ESFA.DC.Ncs.Dss.Service.Interfaces;

namespace ESFA.DC.Ncs.Dss.Service
{
    public class Application
    {
        private readonly ILogger _logger;
        private readonly IDssService<MessageCrossLoadFromNCSDto> _dssService;
        private readonly IExecutionContext _executionContext;

        public Application(
            ILogger logger,
            IDssService<MessageCrossLoadFromNCSDto> dssService,
            IExecutionContext executionContext)
        {
            _logger = logger;
            _dssService = dssService;
            _executionContext = executionContext;
        }

        public void Run()
        {
            _dssService.Subscribe();

            _logger.LogInfo($"Started {_executionContext.JobId}!");

            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            oSignalEvent.WaitOne();
        }
    }
}
