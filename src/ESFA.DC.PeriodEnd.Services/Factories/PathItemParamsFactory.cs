using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Services.Factories
{
    public class PathItemParamsFactory : IPathItemParamsFactory
    {
        public PathItemParams GetPathItemParams(int ordinal, int collectionYear, int period, string collectionName, int pathId)
        {
            return new PathItemParams
            {
                Ordinal = ordinal,
                CollectionYear = collectionYear,
                Period = period,
                CollectionName = collectionName,
                PathId = pathId
            };
        }
    }
}