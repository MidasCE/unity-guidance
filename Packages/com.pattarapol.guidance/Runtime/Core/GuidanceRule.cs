using System;
using System.Collections.Generic;
using System.Linq;

namespace Guidance.Core
{
    public sealed class GuidanceRule
    {
        public string Id { get; }
        public IReadOnlyList<ICondition> Conditions { get; }
        public GuidancePrompt Prompt { get; }

        /// <summary>Higher runs first. Equal priorities keep declaration order.</summary>
        public int Priority { get; }

        public bool ShowOnce { get; }

        public GuidanceRule(string id, GuidancePrompt prompt, IEnumerable<ICondition> conditions = null,
            int priority = 0, bool showOnce = true)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Rule id is required.", nameof(id));

            Id = id;
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            Conditions = (conditions ?? Enumerable.Empty<ICondition>()).ToArray();
            Priority = priority;
            ShowOnce = showOnce;
        }

        public bool IsSatisfiedBy(IGuidanceContext context) => Conditions.All(c => c.Evaluate(context));
    }
}
