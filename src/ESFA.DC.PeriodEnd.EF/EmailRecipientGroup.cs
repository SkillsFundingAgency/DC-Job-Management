namespace ESFA.DC.PeriodEnd.EF
{
    public partial class EmailRecipientGroup
    {
        public int EmailRecipientGroupId { get; set; }
        public int EmailId { get; set; }
        public int? RecipientGroupId { get; set; }

        public virtual Email Email { get; set; }
        public virtual RecipientGroup RecipientGroup { get; set; }
    }
}
