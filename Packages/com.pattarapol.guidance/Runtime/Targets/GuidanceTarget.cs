using UnityEngine;

namespace Guidance
{
    /// <summary>
    /// Tags a GameObject so guidance rules can point at it by id.
    /// Works on UI (RectTransform) and on world objects (2D sprites, 3D meshes).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GuidanceTarget : MonoBehaviour, IGuidanceTarget
    {
        [SerializeField] string id;

        [Tooltip("Camera used for world-space objects. Defaults to Camera.main.")]
        [SerializeField] Camera worldCamera;

        static readonly Vector3[] Corners = new Vector3[8];

        Renderer[] renderers;

        public string Id => id;

        void OnEnable()
        {
            renderers = GetComponentsInChildren<Renderer>();
            GuidanceTargetRegistry.Default.Register(this);
        }

        void OnDisable() => GuidanceTargetRegistry.Default.Unregister(this);

        public bool TryGetScreenRect(out Rect screenRect)
        {
            var found = transform is RectTransform rectTransform
                ? TryGetUiRect(rectTransform, out screenRect)
                : TryGetWorldRect(out screenRect);

            return found && screenRect.Overlaps(new Rect(0, 0, Screen.width, Screen.height));
        }

        static bool TryGetUiRect(RectTransform rectTransform, out Rect screenRect)
        {
            screenRect = default;
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null || !canvas.isActiveAndEnabled)
                return false;

            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            rectTransform.GetWorldCorners(Corners);
            for (var i = 0; i < 4; i++)
                Corners[i] = RectTransformUtility.WorldToScreenPoint(camera, Corners[i]);

            screenRect = Enclose(Corners, 4);
            return true;
        }

        bool TryGetWorldRect(out Rect screenRect)
        {
            screenRect = default;
            var camera = worldCamera != null ? worldCamera : Camera.main;
            if (camera == null)
                return false;

            if (!TryGetWorldBounds(out var bounds))
            {
                // No renderer: point at the pivot.
                var point = camera.WorldToScreenPoint(transform.position);
                if (point.z < 0) return false;
                screenRect = new Rect(point.x, point.y, 0, 0);
                return true;
            }

            var min = bounds.min;
            var max = bounds.max;
            var count = 0;
            for (var i = 0; i < 8; i++)
            {
                var corner = new Vector3(
                    (i & 1) == 0 ? min.x : max.x,
                    (i & 2) == 0 ? min.y : max.y,
                    (i & 4) == 0 ? min.z : max.z);
                var screen = camera.WorldToScreenPoint(corner);
                if (screen.z >= 0)
                    Corners[count++] = screen;
            }

            if (count == 0)
                return false;

            screenRect = Enclose(Corners, count);
            return true;
        }

        bool TryGetWorldBounds(out Bounds bounds)
        {
            bounds = default;
            var any = false;
            foreach (var r in renderers)
            {
                if (r == null || !r.enabled || !r.gameObject.activeInHierarchy) continue;
                if (any) bounds.Encapsulate(r.bounds);
                else { bounds = r.bounds; any = true; }
            }

            return any;
        }

        static Rect Enclose(Vector3[] points, int count)
        {
            var min = (Vector2)points[0];
            var max = min;
            for (var i = 1; i < count; i++)
            {
                min = Vector2.Min(min, points[i]);
                max = Vector2.Max(max, points[i]);
            }

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
