using System.Collections.Generic;
using Guidance.Core;
using UnityEngine;

namespace Guidance
{
    public sealed class PlayerPrefsProgressStore : IGuidanceProgressStore
    {
        const char Separator = '\n';

        readonly string key;
        readonly HashSet<string> seen;

        /// <param name="key">PlayerPrefs key. Use a per-user key if your game has multiple accounts on one device.</param>
        public PlayerPrefsProgressStore(string key = "guidance.seen")
        {
            this.key = key;
            var stored = PlayerPrefs.GetString(key, string.Empty);
            seen = new HashSet<string>(stored.Split(new[] { Separator }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public bool HasSeen(string ruleId) => seen.Contains(ruleId);

        public void MarkSeen(string ruleId)
        {
            if (seen.Add(ruleId))
                Save();
        }

        public void ResetAll()
        {
            seen.Clear();
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        void Save()
        {
            PlayerPrefs.SetString(key, string.Join(Separator.ToString(), seen));
            PlayerPrefs.Save();
        }
    }
}
