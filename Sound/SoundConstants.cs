namespace Boids.Sound
{
    internal enum PlayerSoundType
    {
        WingFlap,
        Eat
    }
    internal enum BoidSoundType
    {
        WingFlap,
        Death
    }
    internal enum AmbientSoundType
    {
        Wind,
        Forest
    }
    internal static class SoundConstants
    {
        public const float FaderStartDistanc = 100f;
        public const float FaderEndDistance = 500f;
        public const float DefaultMasterVolume = 0.5f;
        public const float DefaultSfxVolume = 0.5f;
        public const float DefaultAmbientVolume = 0.3f;

    }
}
