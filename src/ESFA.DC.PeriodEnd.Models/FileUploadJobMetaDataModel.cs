namespace ESFA.DC.PeriodEnd.Models
{
    public class FileUploadJobMetaDataModel
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public string FileName { get; set; }

        public decimal? FileSize { get; set; }

        public string StorageReference { get; set; }

        public int PeriodNumber { get; set; }
    }
}