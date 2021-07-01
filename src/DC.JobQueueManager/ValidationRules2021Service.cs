using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.JobManagement.Constants;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace ESFA.DC.JobQueueManager
{
    public class ValidationRules2021Service : AbstractValidationRulesService, IValidationRulesService
    {
        private readonly Func<IIlr2021RulebaseContext> _ilr2021RulebaseContext;

        public ValidationRules2021Service(Func<IIlr2021RulebaseContext> ilr2021RulebaseContext, Func<IValidationMessagesContext> validationMessagesContext)
            : base(validationMessagesContext)
        {
            _ilr2021RulebaseContext = ilr2021RulebaseContext;
        }

        public int AcademicYear => AcademicYearConstants.Y2021;

        public async Task<IEnumerable<ValidationRuleDto>> GetILRValidationRulesAsync(CancellationToken cancellationToken)
        {
            HashSet<string> triggeredRules;

            using (var context = _ilr2021RulebaseContext())
            {
                var rules = await context.ValidationErrors.Select(x => x.RuleName).Distinct().ToListAsync();
                triggeredRules = rules.ToHashSet();
            }

            var allRules = await GetAllValidationRulesForYearAsync(AcademicYear, cancellationToken);

            return RulesList(allRules, triggeredRules);
        }
    }
}
