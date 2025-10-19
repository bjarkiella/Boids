using System;
using System.Collections.Generic;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{

    internal class ParallaxLayer
    {
        private readonly List<CloudEntity> _clouds = [];
        private readonly Texture2D _cloudTexture;
        private readonly List<Rectangle> _cloudFrames;
        private readonly float _speedMultiplier;  // 0.2f for far, 0.5f for mid, 1.0f for near
        private readonly float _spawnRate;        // How often to spawn clouds
        private float _timeSinceLastSpawn;

        public ParallaxLayer(Texture2D texture, List<Rectangle> frames, float speedMultiplier, float spawnRate)
        {

        }

        public void Update()
        {
            // Update all clouds with this layer's speed
            // Check for spawning new clouds
            // Remove off-screen clouds
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw all clouds in this layer
        }

        private void SpawnCloud()
        {
            // Random cloud from frames
            // Random Y position
            // Spawn off-screen (left or right depending on direction)
        }
    }
}
