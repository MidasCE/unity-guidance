using System.Collections.Generic;

namespace Guidance.Core
{
    public sealed class InMemoryProgressStore : IGuidanceProgressStore
    {
        readonly HashSet<string> seen = new HashSet<string>();

        public bool HasSeen(string ruleId) => seen.Contains(ruleId);
        public void MarkSeen(string ruleId) => seen.Add(ruleId);
        public void ResetAll() => seen.Clear();
    }
}
