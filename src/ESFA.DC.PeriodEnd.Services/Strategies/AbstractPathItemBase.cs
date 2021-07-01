using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public class AbstractPathItemBase : IILRPathItem, INCSPathItem
    {
        public virtual string DisplayName => null;

        public virtual string ReportFileName => null;

        public virtual int PathItemId { get; }

        public virtual List<int> ItemSubPaths { get; }

        public virtual bool IsPausing { get; }

        public virtual int EmailPathItemId { get; }

        public virtual bool IsHidden => false;

        public virtual bool IsInitiating => false;

        public string CollectionNameInDatabase => CollectionName;

        public virtual PeriodEndEntityType EntityType => PeriodEndEntityType.PathItem;

        protected virtual string CollectionName => null;

        public virtual Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<bool> IsValidForPeriod(int period, int collectionYear, IDictionary<string, IEnumerable<int>> validities)
        {
            throw new System.NotImplementedException();
        }

        protected virtual string GetValidityKey()
        {
            return $"{PathItemId}-{EntityType}";
        }
    }
}
