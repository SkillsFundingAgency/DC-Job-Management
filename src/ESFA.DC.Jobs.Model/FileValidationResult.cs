namespace ESFA.DC.Jobs.Model
{
    public class FileValidationResult
    {
        public int TotalLearners { get; set; }

        public int TotalErrors { get; set; }

        public int TotalWarnings { get; set; }

        public int TotalErrorLearners { get; set; }

        public int TotalWarningLearners { get; set; }

        public string ErrorMessage { get; set; }

        public int TotalDataMatchLearners { get; set; }

        public int TotalDataMatchErrors { get; set; }

        public bool IsSchemaError { get; set; }

        public int? PreviouslySubmittedLearnerCount { get; set; }
    }
}
