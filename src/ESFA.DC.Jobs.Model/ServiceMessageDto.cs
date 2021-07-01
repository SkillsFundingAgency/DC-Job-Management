using System;
using System.Collections.Generic;

namespace ESFA.DC.Jobs.Model
{
    public class ServiceMessageDto
    {
        public int Id { get; set; }

        public string Headline { get; set; }

        public string Message { get; set; }

        public DateTime StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public bool IsEnabled { get; set; }

        public IEnumerable<ServicePageDto> Pages { get; set; }
    }
}
