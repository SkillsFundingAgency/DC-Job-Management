using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class Mcadetail
    {
        public int Id { get; set; }
        public long Ukprn { get; set; }
        public string Glacode { get; set; }
        public int Sofcode { get; set; }
    }
}
