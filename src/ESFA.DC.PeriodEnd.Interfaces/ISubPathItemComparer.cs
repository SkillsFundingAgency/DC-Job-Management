using System.Linq;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface ISubPathItemComparer
    {
        IOrderedEnumerable<IPathItem> ConvertToBasePathItem<T>(IOrderedEnumerable<T> pathItems)
            where T : IPathItem;
    }
}