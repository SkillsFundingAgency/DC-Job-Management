using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.JobManagement.Constants;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace ESFA.DC.JobQueueManager
{
    public class ValidationRules1819Service : AbstractValidationRulesService, IValidationRulesService
    {
        private readonly Func<IIlr1819RulebaseContext> _ilr1819RulebaseContext;

        public ValidationRules1819Service(Func<IIlr1819RulebaseContext> ilr1819RulebaseContext, Func<IValidationMessagesContext> validationMessagesContext)
            : base(validationMessagesContext)
        {
            _ilr1819RulebaseContext = ilr1819RulebaseContext;
        }

        public int AcademicYear => AcademicYearConstants.Y1819;

        public async Task<IEnumerable<ValidationRuleDto>> GetILRValidationRulesAsync(CancellationToken cancellationToken)
        {
            HashSet<string> triggeredRules;

            using (var context = _ilr1819RulebaseContext())
            {
                var rules = await context.ValidationErrors.Select(x => x.RuleName).Distinct().ToListAsync();
                triggeredRules = rules.ToHashSet();
            }

            var allRules = await GetAllValidationRulesForYearAsync(AcademicYear, cancellationToken);

            return RulesList(allRules, triggeredRules);
        }
    }
}
