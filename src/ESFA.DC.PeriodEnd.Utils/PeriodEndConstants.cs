using System;
using System.Collections.Generic;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.PeriodEnd.Utils
{
    public enum PeriodEndValidityState
    {
        Unchecked = 0,
        Checked = 1,
        Disabled = 2
    }

    public class PeriodEndConstants
    {
        public const string IlrCollectionNamePrefix = "ILR";
        public const string NcsCollectionNamePrefix = "NCS";
        public const string GenericCollectionType = "Generic";

        public const char NCSReturnPeriodPrefix = 'N';
        public const char ILRReturnPeriodPrefix = 'R';
        public const string CollectionNameYearToken = "{Year}";

        public const string UkprnToken = "{UKPRN}";

        public const string IlrPeriodPrefix = "R";
        public const string NcsPeriodPrefix = "N";
        public const string AllfPeriodPrefix = "A";

        public const string CollectionName_ReferenceDataEPA = "REF-EPA";
        public const string CollectionName_ReferenceDataFCS = "REF-FCS";
        public const string CollectionName_ReferenceDataEDRS = "REF-EDRS";

        public const string CollectionName_ReferenceDataILRLookups2122 = "REF-ILRLookups2122";

        public static readonly string LearnerLevelViewReportFileName = $"{UkprnToken}-LLVSample.zip";

        public static readonly string CollectionName_ILRSubmission = $"{IlrCollectionNamePrefix}{CollectionNameYearToken}";

        public static readonly string CollectionName_DataQualityReport = $"PE-DataQuality-Report{CollectionNameYearToken}";
        public static readonly string CollectionName_DataExtractReport = $"PE-DataExtract-Report{CollectionNameYearToken}";
        public static readonly string CollectionName_CollectionStats = $"PE-Collection-Stats{CollectionNameYearToken}";

        public static readonly string CollectionName_DasStart = $"PE-DAS-Start{CollectionNameYearToken}";
        public static readonly string CollectionName_DasRun = $"PE-DAS-Run{CollectionNameYearToken}";
        public static readonly string CollectionName_DasEnd = $"PE-DAS-Stop{CollectionNameYearToken}";
        public static readonly string CollectionName_DasSubmissionWindowPeriodValidation = $"PE-DAS-SubmissionWindowPeriodValidation{CollectionNameYearToken}";

        public static readonly string CollectionName_DasNonSubmitting = $"PE-DAS-Submission{CollectionNameYearToken}";

        public static readonly string CollectionName_ESFSummarisation = $"PE-ESF-Summarisation{CollectionNameYearToken}";
        public static readonly string CollectionName_DCSummarisation = $"PE-DC-Summarisation{CollectionNameYearToken}";
        public static readonly string CollectionName_NCSSummarisation = $"PE-NCS-Summarisation{CollectionNameYearToken}";
        public static readonly string CollectionName_AppSummarisation = $"PE-App-Summarisation{CollectionNameYearToken}";

        public static readonly string CollectionName_ALLFSummarisation = $"PE-ALLF-Summarisation{CollectionNameYearToken}";

        public static readonly string CollectionName_AppsAdditionalPaymentsReport = $"PE-DAS-AppsAdditionalPaymentsReport{CollectionNameYearToken}";
        public static readonly string CollectionName_AppsMonthlyPaymentReport = $"PE-DAS-AppsMonthlyPaymentReport{CollectionNameYearToken}";
        public static readonly string CollectionName_FundingSummaryPeriodEndReport = $"PE-DAS-FundingSummaryPeriodEndReport{CollectionNameYearToken}";
        public static readonly string CollectionName_AppsCoInvestmentContributionsReport = $"PE-DAS-AppsCoInvestmentContributionsReport{CollectionNameYearToken}";

        public static readonly string CollectionName_AppsDataMatchMonthEndReport = $"PE-DAS-AppsDataMatchMonthEndReport{CollectionNameYearToken}";
        public static readonly string CollectionName_AppsInternalDataMatchMonthEndReport = $"PE-DAS-AppsInternalDataMatchMonthEndReport{CollectionNameYearToken}";
        public static readonly string CollectionName_EsfaNonContractedDevolvedAdultEducationActivityReport = $"PE-ESFANonContDevolvedAdultEducationReport{CollectionNameYearToken}";

        public static readonly string CollectionName_CrossYearIndicativePaymentsReport = $"PE-DAS-CrossYearPaymentsReport{CollectionNameYearToken}";

        public static readonly string CollectionName_DasPeriodEndReports = $"PE-DAS-PeriodEndReportPreparation{CollectionNameYearToken}";

        public static readonly string CollectionName_McaReport = $"PE-MCA-Reports{CollectionNameYearToken}";

        public static readonly string CollectionName_DataWarehouse1 = $"PE-Data-Warehouse1{CollectionNameYearToken}";
        public static readonly string CollectionName_DataWarehouse2 = $"PE-Data-Warehouse2{CollectionNameYearToken}";
        public static readonly string CollectionName_StandardFile = $"PE-StandardFile{CollectionNameYearToken}";

        public static readonly string CollectionName_ProviderSubmissionsReport = $"PE-ProviderSubmissions-Report{CollectionNameYearToken}";

        public static readonly string CollectionName_ActCountReport = $"PE-ACT-Count-Report{CollectionNameYearToken}";

        public static readonly string CollectionName_LLVReport = $"PE-UYP-LLVReport{CollectionNameYearToken}";

        public static readonly string CollectionName_DAS_ReportsFinished = $"PE-DAS-Reports-Finished{CollectionNameYearToken}";

        public static readonly string CollectionName_NCSDataExtractReport = $"PE-NCS-DataExtract-Report{CollectionNameYearToken}";

        public static readonly List<int> JobFinished = new List<int>
        {
            Convert.ToInt32(JobStatusType.Completed),
            Convert.ToInt32(JobStatusType.FailedRetry),
            Convert.ToInt32(JobStatusType.Failed)
        };

        public static readonly List<string> ReferenceDataJobsToHold = new List<string>
        {
            CollectionName_ReferenceDataEPA,
            CollectionName_ReferenceDataFCS,
            CollectionName_ReferenceDataEDRS,
            CollectionName_ReferenceDataILRLookups2122
        };

        public static readonly List<long> SampleReportProviderUkPrns = new List<long>
        {
            10003915,
            10004177,
            10033440,
            10003161
        };

        public static readonly List<int> CriticalPaths = new List<int>
        {
            (int)PeriodEndPath.ILRCriticalPath,
            (int)PeriodEndPath.NCSCriticalPath,
            (int)PeriodEndPath.ALLFCriticalPath
        };
    }
}