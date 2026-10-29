using System.Collections.Generic;

namespace Guidance.Core
{
    public sealed class GuidanceContext : IGuidanceContext
    {
        readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public GuidanceContext Set<T>(string key, T value)
        {
            values[key] = value;
            return this;
        }

        public bool Remove(string key) => values.Remove(key);

        public bool TryGet<T>(string key, out T value)
        {
            if (values.TryGetValue(key, out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }
    }
}
