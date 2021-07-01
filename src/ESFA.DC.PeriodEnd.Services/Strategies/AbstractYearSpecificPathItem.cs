using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public class AbstractYearSpecificPathItem : AbstractPathItemBase
    {
        protected virtual string GetYearSpecificCollectionName(int collectionYear)
        {
            return GetYearSpecificCollectionName(collectionYear, CollectionName);
        }

        protected virtual string GetYearSpecificCollectionName(int collectionYear, string collectionName)
        {
            var year = string.Empty;
            if (collectionYear != 0)
            {
                year = collectionYear.ToString();
            }

            return collectionName.Replace(PeriodEndConstants.CollectionNameYearToken, year);
        }
    }
}