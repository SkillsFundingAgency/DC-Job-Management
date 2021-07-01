using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractBlockingEmailTask : AbstractEmailTaskBase
    {
        protected AbstractBlockingEmailTask(
            ILogger logger,
            IEmailService emailService,
            IStateService stateService,
            IPathItemReturnFactory returnFactory)
            : base(logger, emailService, stateService, returnFactory)
        {
        }

        public override bool IsPausing => true;

        protected override bool IsBlockingTask => true;
    }
}