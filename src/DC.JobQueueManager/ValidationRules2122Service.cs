using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR2122.DataStore.EF.Interface;
using ESFA.DC.JobManagement.Constants;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace ESFA.DC.JobQueueManager
{
    public class ValidationRules2122Service : AbstractValidationRulesService, IValidationRulesService
    {
        private readonly Func<IIlr2122Context> _ilrContext;

        public ValidationRules2122Service(Func<IIlr2122Context> ilrContext, Func<IValidationMessagesContext> validationMessagesContext)
            : base(validationMessagesContext)
        {
            _ilrContext = ilrContext;
        }

        public int AcademicYear => AcademicYearConstants.Y2122;

        public async Task<IEnumerable<ValidationRuleDto>> GetILRValidationRulesAsync(CancellationToken cancellationToken)
        {
            HashSet<string> triggeredRules;

            using (var context = _ilrContext())
            {
                var rules = await context.ValidationErrors.Select(x => x.RuleName).Distinct().ToListAsync();
                triggeredRules = rules.ToHashSet();
            }

            var allRules = await GetAllValidationRulesForYearAsync(AcademicYear, cancellationToken);

            return RulesList(allRules, triggeredRules);
        }
    }
}
