namespace ESFA.DC.Jobs.Model
{
    using System;

    [Serializable]
    public class ValidationRuleDetailsReportJob : Job
    {
        public string Rule { get; set; }

        public int SelectedCollectionYear { get; set; }
    }
}
