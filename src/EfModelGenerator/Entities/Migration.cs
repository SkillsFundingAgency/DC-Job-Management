using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Migration
    {
        public Guid MigrationId { get; set; }
        public string Description { get; set; }
        public DateTime DateTimeCreatedUtc { get; set; }
        public string Author { get; set; }
        public string BuildBuildnumber { get; set; }
        public string BuildBranchname { get; set; }
        public string ReleaseReleasename { get; set; }
    }
}
