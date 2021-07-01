using System.Text.RegularExpressions;

namespace ESFA.DC.PeriodEnd.Services
{
    public static class RegexDefinitions
    {
        public static Regex ReportDate = new Regex(@"\d{8}-\d{6}", RegexOptions.Compiled);
    }
}