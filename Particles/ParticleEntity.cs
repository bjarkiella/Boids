using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Particles
{
    internal class ParticleEntity(
            Animation animation,
            Vector2 position,
            Vector2 velocity,
            float lifetime,
            bool isGravity,
            float rotation)

        :BaseEntity(animation.Texture, position,velocity, 0f,animation)
    {
        private readonly float _lifetime = lifetime;
        private float _age = 0f;
        private readonly bool _isGravity = isGravity;
        private readonly Animation _animation = animation;
        public bool IsDead => _age >= _lifetime;
        private readonly float _rotation = rotation;

        public void Update()
        {
            _age += Dt;
            _animation.Update();
            Position += Velocity * Dt;
            if (_isGravity)
                Velocity += new Vector2(0,50f) * Dt;
        }
        public void Draw(SpriteBatch sb)
        {

            if (IsDead) return;

            float alpha = 1.0f - (_age / _lifetime);
            Color color = Color.White * alpha;
            float scale = Utils.RandomFloatRange(0.5f,3.0f);

            Vector2 origin = new(_animation.FrameWidth/2f, _animation.FrameHeight/2f);
            sb.Draw(_animation.Texture, Position, _animation.CurrentFrame, color,_rotation,origin,scale,SpriteEffects.None,0f);

        }
    }
}

