using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class AbstractValidationRulesService
    {
        private readonly Func<IValidationMessagesContext> _validationMessagesContext;

        public AbstractValidationRulesService(Func<IValidationMessagesContext> validationMessagesContext)
        {
            _validationMessagesContext = validationMessagesContext;
        }

        public async Task<IEnumerable<string>> GetAllValidationRulesForYearAsync(int academicYear, CancellationToken cancellationToken)
        {
            using (var context = _validationMessagesContext())
            {
                var ruleNames = await context
                    .Messages
                    .Where(x => x.CollectionYear == academicYear)
                    .Select(x => x.RuleName).ToListAsync(cancellationToken);

                return ruleNames;
            }
        }

        public IEnumerable<ValidationRuleDto> RulesList(IEnumerable<string> allRules, HashSet<string> triggeredRules)
        {
            List<ValidationRuleDto> result = new List<ValidationRuleDto>();

            foreach (var rule in allRules)
            {
                result.Add(new ValidationRuleDto() { RuleName = rule, IsTriggered = triggeredRules.Contains(rule) });
            }

            return result;
        }
    }
}
