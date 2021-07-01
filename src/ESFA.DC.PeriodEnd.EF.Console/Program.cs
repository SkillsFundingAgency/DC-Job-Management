namespace ESFA.DC.PeriodEnd.EF.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //dotnet.exe ef dbcontext scaffold "Server=.\;Database=JobManagement;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities -c PeriodEndContext --table PeriodEnd.PeriodEnd --table PeriodEnd.Path --table PeriodEnd.PathItem --table PeriodEnd.PathItemJob --table PeriodEnd.EmailValidityPeriod --table PeriodEnd.ValidityPeriod --table Mailing.Recipient --table Mailing.RecipientGroup --table Mailing.RecipientGroupRecipient --table Mailing.Email --table Mailing.EmailRecipientGroup --table dbo.Job --table dbo.ReturnPeriod --table dbo.Collection --table dbo.CollectionType --table dbo.Schedule --table dbo.MCADetail --force
        }
    }
}