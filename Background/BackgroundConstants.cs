
namespace Boids.Background
{
    internal static class BackgroundConstants
    {
        internal const float BaseCloudSpeed = 20f;      // Base pixels per second
        internal const float FarLayerSpeed = 0.3f;      // 30% of base speed
        internal const float MidLayerSpeed = 0.6f;      // 60% of base speed
        internal const float NearLayerSpeed = 1.0f;     // 100% of base speed

        internal const float FarLayerSpawnRate = 5.0f;  // Seconds between spawns
        internal const float MidLayerSpawnRate = 3.0f;
        internal const float NearLayerSpawnRate = 2.0f;

        internal const float FarLayerScale = 0.5f;      // Smaller = farther
        internal const float MidLayerScale = 0.75f;
        internal const float NearLayerScale = 1.0f;

        internal const float FarLayerAlpha = 0.4f;      // Transparency for depth
        internal const float MidLayerAlpha = 0.6f;
        internal const float NearLayerAlpha = 0.8f;
    }
}
