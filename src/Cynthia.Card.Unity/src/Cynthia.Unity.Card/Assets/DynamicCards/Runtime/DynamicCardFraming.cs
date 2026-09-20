using UnityEngine;

namespace Assets.Script.DynamicCards
{
    internal static class DynamicCardFraming
    {
        // CardRTRenderer/Card/Appereance is at (0, 2, 0) in Legacy2017,
        // Thronebreaker and Latest. CameraValuesChanger changes Z and FOV only.
        internal static readonly Vector3 AppearanceOffset = new Vector3(0f, 2f, 0f);

        // These source scenes have no CameraValuesChanger. Their old importer
        // substituted guessed defaults; use the renderer prefab's defaults instead.
        private static bool UsesRendererDefaults(DynamicCardEntry entry)
        {
            return entry.id == "13240100" || entry.id == "15560101" || entry.id == "19220101";
        }

        public static float CameraDistance(DynamicCardEntry entry, bool thumbnail)
        { return !thumbnail && UsesRendererDefaults(entry) ? -30f : entry.cameraDistance; }

        public static Rect ThumbnailRegion(Vector2 displaySize)
        {
            var region = new Rect(.185f, .52f, .633f, .19f);
            if (displaySize.x <= 0 || displaySize.y <= 0) return region;
            float aspect = displaySize.x / displaySize.y;
            float width = Mathf.Min(region.width, region.height * aspect);
            float height = width / aspect;
            return new Rect(region.center.x - width * .5f, region.center.y - height * .5f, width, height);
        }

        public static Rect ArtRegion(DynamicCardEntry entry)
        {
            // Project the old client's Card_Default/ElementName_ImagePlane into
            // its default camera (FOV 25.61, Z -30). The full static artwork uses
            // this rectangle; all imported premiums share that display coordinate system.
            return new Rect(.16602942f, .0035620682f, .68260696f, .98114324f);
        }

        public static void Apply(Camera camera, DynamicCardEntry entry, bool thumbnail = false)
        {
            if (!thumbnail && UsesRendererDefaults(entry))
            {
                camera.fieldOfView = 25.61000061f;
                camera.nearClipPlane = entry.id == "13240100" ? .01f : 5f;
                camera.farClipPlane = 300f;
            }
            camera.aspect = 1f;
            camera.ResetProjectionMatrix();
            if (thumbnail)
            {
                // The narrow deck strip has its own existing focal crop. Preserve it
                // independently of the full portrait's source-camera restoration.
                float height = (947f - entry.topMargin) / 1024f;
                float offset = Mathf.Max(0f, .13f * height - entry.topMargin / 1024f);
                var projection = camera.projectionMatrix;
                projection.m12 = -2f * (offset + entry.thumbnailFramingCorrection);
                camera.projectionMatrix = projection;
            }
        }
    }
}
