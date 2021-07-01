using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractEmailTask : AbstractEmailTaskBase
    {
        protected AbstractEmailTask(
            ILogger logger,
            IEmailService emailService,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
        }

        protected override bool IsBlockingTask => false;
    }
}