using System.Collections.Generic;
using System.Linq;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class BaseTest
    {
        protected IDictionary<string, IEnumerable<int>> BuildValidPeriods(int year)
        {
            Dictionary<string, IEnumerable<int>> ret = new Dictionary<string, IEnumerable<int>>();

            ret[GetYearName(PeriodEndConstants.CollectionName_ActCountReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_CollectionStats, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppSummarisation, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppsAdditionalPaymentsReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppsCoInvestmentContributionsReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppsDataMatchMonthEndReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppsInternalDataMatchMonthEndReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_AppsMonthlyPaymentReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DCSummarisation, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DasEnd, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DasNonSubmitting, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DasPeriodEndReports, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DasRun, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DasStart, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DataExtractReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DataQualityReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DataWarehouse1, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_DataWarehouse2, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_ESFSummarisation, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_FundingSummaryPeriodEndReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_McaReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_ProviderSubmissionsReport, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_ReferenceDataEPA, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_ReferenceDataFCS, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_StandardFile, year)] = Enumerable.Range(1, 14);
            ret[GetYearName(PeriodEndConstants.CollectionName_EsfaNonContractedDevolvedAdultEducationActivityReport, year)] = Enumerable.Range(1, 14);

            return ret;
        }

        private string GetYearName(string collectionName, int year)
        {
            return collectionName.Replace(PeriodEndConstants.CollectionNameYearToken, year.ToString());
        }
    }
}
