using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{
    internal class ParallaxEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, float speed, float scale)
    {
        private readonly Texture2D _texture = texture;
        private readonly Rectangle _sourceRect = sourceRect;
        private Vector2 _position = position;
        private readonly float _baseSpeed = speed;
        private readonly float _scale = scale;

        public float Width => _sourceRect.Width*_scale;
        public float Height => _sourceRect.Height*_scale;

        protected static float Dt => Time.Delta;

        public void Update(float layerSpeedMultiplier)
        {
            _position.X += _baseSpeed * layerSpeedMultiplier * Dt;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 origin = new (_sourceRect.Width/2f, _sourceRect.Height/2f);
            sb.Draw(_texture, _position, _sourceRect, Color.White, 0f, origin, _scale , SpriteEffects.None,0f);
        }

        public Rectangle GetBounds()
        {
            Rectangle EntityBox = new (
                    (int)(_position.X - Width/2f),
                    (int)(_position.Y - Height/2f),
                    (int)Width,
                    (int)Height);
            return EntityBox;
        }

        public bool IsOffScreen()
        {
            float halfWidth = _sourceRect.Width * _scale/2f;
            if (_position.X - halfWidth > Constants.ActiveWidth || _position.X + halfWidth < 0)
                return true;
            else 
                return false;
        }
    }
}
