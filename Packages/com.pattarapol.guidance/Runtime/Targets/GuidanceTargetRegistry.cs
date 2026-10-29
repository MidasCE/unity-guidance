using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guidance
{
    public sealed class GuidanceTargetRegistry : IGuidanceTargetResolver
    {
        public static GuidanceTargetRegistry Default { get; } = new GuidanceTargetRegistry();

        readonly Dictionary<string, List<IGuidanceTarget>> targets = new Dictionary<string, List<IGuidanceTarget>>();

        public void Register(IGuidanceTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(target.Id)) return;

            if (!targets.TryGetValue(target.Id, out var list))
                targets[target.Id] = list = new List<IGuidanceTarget>();

            if (!list.Contains(target))
                list.Add(target);
        }

        public void Unregister(IGuidanceTarget target)
        {
            if (target == null || string.IsNullOrEmpty(target.Id)) return;
            if (targets.TryGetValue(target.Id, out var list) && list.Remove(target) && list.Count == 0)
                targets.Remove(target.Id);
        }

        /// <summary>
        /// Several objects may share an id (e.g. list items); the most recently
        /// registered one that is actually on screen wins.
        /// </summary>
        public bool TryResolve(string targetId, out IGuidanceTarget target)
        {
            target = null;
            if (string.IsNullOrEmpty(targetId) || !targets.TryGetValue(targetId, out var list))
                return false;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].TryGetScreenRect(out _))
                {
                    target = list[i];
                    return true;
                }
            }

            return false;
        }
    }
}
