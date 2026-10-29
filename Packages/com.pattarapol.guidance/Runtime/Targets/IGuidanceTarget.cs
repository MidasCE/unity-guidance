using UnityEngine;

namespace Guidance
{
    /// <summary>
    /// Anything guidance can point at. Every target, whether UI, 2D or 3D,
    /// reduces to a rect in screen pixels (origin bottom-left), so the view
    /// never needs to know what it's highlighting.
    /// </summary>
    public interface IGuidanceTarget
    {
        string Id { get; }

        /// <summary>False when the target isn't currently visible on screen.</summary>
        bool TryGetScreenRect(out Rect screenRect);
    }
}
