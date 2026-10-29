using System;

namespace Guidance.Core
{
    /// <summary>What to show and where. The view layer decides how.</summary>
    public sealed class GuidancePrompt
    {
        public string TargetId { get; }
        public string Text { get; }
        public Placement Placement { get; }

        /// <summary>Optional view style key; null means the default style.</summary>
        public string Style { get; }

        public GuidancePrompt(string targetId, string text, Placement placement = Placement.Auto, string style = null)
        {
            if (string.IsNullOrEmpty(targetId))
                throw new ArgumentException("Target id is required.", nameof(targetId));

            TargetId = targetId;
            Text = text ?? string.Empty;
            Placement = placement;
            Style = style;
        }
    }
}
