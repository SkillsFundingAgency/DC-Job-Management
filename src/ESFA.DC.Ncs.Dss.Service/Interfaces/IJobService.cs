using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.Ncs.Dss.Service.Interfaces
{
    public interface IJobService
    {
        Task<long> SubmitJob(NcsJob ncsJob, CancellationToken cancellationToken = default(CancellationToken));

        Task<Collection> GetNcsCollectionByPeriodDate(DateTime dateInMessage, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReturnPeriod>> GetNcsCollectionPeriodsByCollection(int collectionId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
