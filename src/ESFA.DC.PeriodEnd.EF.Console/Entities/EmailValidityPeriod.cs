using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class EmailValidityPeriod
    {
        public int HubEmailId { get; set; }
        public int Period { get; set; }
        public bool? Enabled { get; set; }
    }
}
