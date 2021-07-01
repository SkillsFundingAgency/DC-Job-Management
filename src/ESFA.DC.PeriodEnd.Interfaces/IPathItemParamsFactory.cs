using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPathItemParamsFactory
    {
        PathItemParams GetPathItemParams(int ordinal, int collectionYear, int period, string collectionName, int pathId);
    }
}