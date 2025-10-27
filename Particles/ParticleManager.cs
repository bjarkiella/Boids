using System.Collections.Generic;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Particles
{
    internal class ParticleManager(Animation particleAnimation)
    {
        // private readonly Texture2D _particleTexture = particleAnimation.Texture;
        // private readonly List<Rectangle> _frames = particleAnimation.Frames;
        // private readonly float _frameDuration = particleAnimation.FrameDuration;
        
        private readonly List<ParticleEntity> _particles =  [];
        private readonly Animation _particleAnimation = particleAnimation;

        public int Count => _particles.Count;

        public void SpawnParticles(Vector2 position, Vector2 velocity, float lifetime,bool isGravity)
        {
            // Create new animation with random starting frame
            int randomStartFrame = Utils.RandomIntRange(0, _particleAnimation.Frames.Count);
            Animation particleAnim = new (
                    _particleAnimation.Texture,
                    _particleAnimation.Frames,
                    _particleAnimation.FrameDuration,
                    isLooping: true,
                    startFrame: randomStartFrame
                    );
            ParticleEntity particle = new(particleAnim, position,velocity,lifetime,isGravity);
            _particles.Add(particle);

        }
        public void Clear()
        {
            _particles.Clear();
        }

        public void Update()
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update();

                if (_particles[i].IsDead)
                {
                    _particles.RemoveAt(i); // O(1) removal at index
                }
            }}

        public void Draw(SpriteBatch sb)
        {
            foreach(ParticleEntity particle in _particles)
                particle.Draw(sb);
        }
    }
}
