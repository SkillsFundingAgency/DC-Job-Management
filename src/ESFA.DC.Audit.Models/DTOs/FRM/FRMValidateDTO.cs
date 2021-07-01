namespace ESFA.DC.Audit.Models.DTOs.FRM
{
    public class FRMValidateDTO
    {
        public string FrmContainerName{ get; set; }

        public string FrmFolderKey { get; set; }

        public int FrmPeriodNumber { get; set; }

        public string CurrentContainerName { get; set; }
    }
}
