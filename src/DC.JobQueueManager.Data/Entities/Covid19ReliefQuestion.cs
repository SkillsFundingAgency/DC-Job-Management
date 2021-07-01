namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Covid19ReliefQuestion
    {
        public int Id { get; set; }
        public string QuestionNumber { get; set; }
        public string Answer { get; set; }
        public int Covid19ReliefSubmissionId { get; set; }
        public string AnswerType { get; set; }

        public virtual Covid19ReliefSubmission Covid19ReliefSubmission { get; set; }
    }
}
