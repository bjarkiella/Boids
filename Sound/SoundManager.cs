// using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Boids.Sound
{
    internal class SoundCategory
    {
        private readonly Dictionary<string, SoundEffect> _sounds;
        private float _volume;
        public SoundCategory(float initVolume = 1.0f)
        {

        }
        public void RegisterSound(string key, SoundEffect sound)
        {

        }
        public void Play(string key, float volumeMultiplier = 1.0f)
        {

        }
        public void SetVolume(float volume)
        {

        }
        public float Volume => _volume;
    }
}
