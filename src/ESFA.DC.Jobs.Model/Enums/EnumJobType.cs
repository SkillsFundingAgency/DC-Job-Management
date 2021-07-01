namespace ESFA.DC.Jobs.Model.Enums
{
    /// <summary>
    /// The enum representing the possible job types in the system.
    /// This enum must be kept up to date with [JobManagement].[dbo].[JobType]
    /// </summary>
    public enum EnumJobType
    {
        IlrSubmission = 1,
        EasSubmission = 2,
        EsfSubmission = 3,
        NcsSubmission = 4,
        Esf2Submission = 7,
        PeriodEnd = 20,
        ReferenceDataEPA = 40,
        ReferenceDataFCS = 41,
        ReferenceDataULN = 42
    }
}
