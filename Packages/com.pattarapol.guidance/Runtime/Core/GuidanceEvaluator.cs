using System;
using System.Collections.Generic;
using System.Linq;

namespace Guidance.Core
{
    public sealed class GuidanceEvaluator
    {
        readonly IReadOnlyList<GuidanceRule> rules;
        readonly IGuidanceProgressStore progress;

        public GuidanceEvaluator(IEnumerable<GuidanceRule> rules, IGuidanceProgressStore progress)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));

            var list = rules.ToList();
            var duplicate = list.GroupBy(r => r.Id).FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                throw new ArgumentException($"Duplicate guidance rule id '{duplicate.Key}'.", nameof(rules));

            // OrderByDescending is stable, so equal priorities keep declaration order.
            this.rules = list.OrderByDescending(r => r.Priority).ToArray();
        }

        /// <summary>
        /// Rules that may show now, best first. Lazy, so a caller can skip a rule
        /// whose target isn't on screen and take the next one.
        /// </summary>
        public IEnumerable<GuidanceRule> Candidates(IGuidanceContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return rules.Where(r => !(r.ShowOnce && progress.HasSeen(r.Id)) && r.IsSatisfiedBy(context));
        }

        public void MarkShown(GuidanceRule rule)
        {
            if (rule.ShowOnce)
                progress.MarkSeen(rule.Id);
        }
    }
}
