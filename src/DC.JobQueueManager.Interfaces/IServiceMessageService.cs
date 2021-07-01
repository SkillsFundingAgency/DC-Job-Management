using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IServiceMessageService
    {
        Task<string> GetMessageAsync(DateTime dateTimeUtc, string url, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceMessageDto>> GetAllMessagesAsync(CancellationToken cancellationToken);

        Task<bool> SaveMessageAsync(ServiceMessageDto dto, CancellationToken cancellationToken);

        Task<ServiceMessageDto> GetMessageAsync(int messageId, CancellationToken cancellationToken);

        Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken);

        Task<IEnumerable<ServicePageDto>> GetAllPagesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetControllerNamesThatDisplayMessagesAsync(CancellationToken cancellationToken);
    }
}
