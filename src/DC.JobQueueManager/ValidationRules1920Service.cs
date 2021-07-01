using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.JobManagement.Constants;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace ESFA.DC.JobQueueManager
{
    public class ValidationRules1920Service : AbstractValidationRulesService, IValidationRulesService
    {
        private readonly Func<IIlr1920RulebaseContext> _ilr1920RulebaseContext;

        public ValidationRules1920Service(Func<IIlr1920RulebaseContext> ilr1920RulebaseContext, Func<IValidationMessagesContext> validationMessagesContext)
            : base(validationMessagesContext)
        {
            _ilr1920RulebaseContext = ilr1920RulebaseContext;
        }

        public int AcademicYear => AcademicYearConstants.Y1920;

        public async Task<IEnumerable<ValidationRuleDto>> GetILRValidationRulesAsync(CancellationToken cancellationToken)
        {
            HashSet<string> triggeredRules;

            using (var context = _ilr1920RulebaseContext())
            {
                var rules = await context.ValidationErrors.Select(x => x.RuleName).Distinct().ToListAsync();
                triggeredRules = rules.ToHashSet();
            }

            var allRules = await GetAllValidationRulesForYearAsync(AcademicYear, cancellationToken);

            return RulesList(allRules, triggeredRules);
        }
    }
}
