using System.Linq;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;

namespace ESFA.DC.PeriodEnd.Services.Comparers
{
    public class SubPathItemComparer : ISubPathItemComparer
    {
        private static IPathComparer _comparer;

        public SubPathItemComparer(IPathComparer pathComparer)
        {
            _comparer = pathComparer;
        }

        public IOrderedEnumerable<IPathItem> ConvertToBasePathItem<T>(IOrderedEnumerable<T> pathItems)
            where T : IPathItem
        {
            return pathItems
                .Select(x => (IPathItem)x)
                .OrderBy(x => x, _comparer);
        }
    }
}