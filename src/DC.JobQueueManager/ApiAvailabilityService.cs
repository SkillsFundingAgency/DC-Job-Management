using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ApiAvailabilityService : IApiAvailabilityService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IAuditFactory _auditFactory;

        public ApiAvailabilityService(Func<IJobQueueDataContext> contextFactory, IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _auditFactory = auditFactory;
        }

        public async Task<bool> IsApiAvailableAsync(string apiName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var isApiDisabled = await context.ApiAvailability.AnyAsync(x => x.ApiName == apiName && x.Enabled == false);

                if (isApiDisabled)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> SetApiAvailabilityAsync(ApiAvailabilityDto apiAvailabilityDto, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await BuildSetApiAvailabilityFunc(apiAvailabilityDto), context);
                await audit.BeforeAsync(cancellationToken);
                var data = await context.ApiAvailability
                    .Where(x => x.ApiName == apiAvailabilityDto.ApiName && x.Process == apiAvailabilityDto.Process)
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return false;
                }

                data.Enabled = apiAvailabilityDto.Enabled;

                await context.SaveChangesAsync(cancellationToken);

                await audit.AfterAndSaveAsync(cancellationToken);
                return true;
            }
        }

        private async Task<Func<IJobQueueDataContext, Task<SetApiAvailabilityDTO>>> BuildSetApiAvailabilityFunc(ApiAvailabilityDto apiAvailabilityDto)
        {
            return async c => await c.ApiAvailability
                .Select(s => new SetApiAvailabilityDTO()
                {
                    ApiName = s.ApiName,
                    Enabled = s.Enabled,
                    Process = s.Process
                })
                .SingleOrDefaultAsync(s => s.ApiName == apiAvailabilityDto.ApiName && s.Process == apiAvailabilityDto.Process);
        }
    }
}
