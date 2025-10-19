using System;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{
    internal class CloudEntity
    {
        private readonly Texture2D _texture;
        private readonly Rectangle _sourceRect;
        private Vector2 _position;
        private readonly float _baseSpeed;
        private readonly float _scale;

        // Constructor takes texture, which cloud sprite, starting position, speed
        public CloudEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, float speed, float scale)
        {

        }

        public void Update(float layerSpeedMultiplier)
        {
            // Move horizontally based on base speed * layer multiplier
            _position.X += _baseSpeed * layerSpeedMultiplier * Time.Delta;
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw with scale and transparency for depth
        }

        public bool IsOffScreen()
        {
            return true;
            // Check if cloud has scrolled past screen edge
        }
    }
}
