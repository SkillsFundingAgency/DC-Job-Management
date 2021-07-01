using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.DeadLetter.Service.Configuration
{
    public class EmailConfig
    {
        public EmailConfig(string apiKey, string recipients)
        {
            ApiKey = apiKey;

            Recipients = recipients?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() ??
                         Enumerable.Empty<string>().ToList();
        }

        public string ApiKey { get; set; }

        public List<string> Recipients { get; set; }
    }
}