using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BaseEntity 
    {
        private Vector2 _velocity;
        private Vector2 _fallbackHeading = new(1,0);

        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { 
            get => _velocity; 
            set{
                _velocity = value;
                if (_velocity.LengthSquared() > Constants.ZeroCompare) _fallbackHeading = Vector2.Normalize(Velocity);
            } 
        } // Used to check if entity is at, or close to zero speed 
        public float VisionFactor { get; set; }
    
        protected float dt => Time.Delta;
        protected float Angle => MathF.Atan2(Velocity.Y, Velocity.X);
        protected float Speed => Velocity.Length();
        protected float Radius => (float)Texture.Width /2;
        protected float VisionRadius => Radius * VisionFactor;
        protected Vector2 Heading => Speed > Constants.ZeroCompare ? Vector2.Normalize(Velocity) : _fallbackHeading;
        

        public BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            VisionFactor = visionFactor;
        }
    
        public void RotateTowardsDir(Vector2 desiredDir,float dt, float maxRate)
        {
            if (desiredDir.LengthSquared() <= Constants.ZeroCompare) return;

            float desiredAngle = MathF.Atan2(desiredDir.Y, desiredDir.X);
            float delta = MathHelper.WrapAngle(desiredAngle - Angle);
            float maxTurn = maxRate * dt;
            float turn = MathHelper.Clamp(delta, -maxTurn, maxTurn);
            float newAngle = Angle + turn;
            Vector2 dir = Utils.newDirection(turn);
            _fallbackHeading = dir;
            Velocity = dir * Speed;
        }

        public void RotateTowardsDir(float turn){
            float newAngle = Angle + turn;
            Vector2 dir = Utils.newDirection(turn);

            Velocity = dir * Speed;
        }

        public void Integrate()
        {
            Position += Velocity * dt;
        }
    }
}
