using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BaseEntity 
    {
        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Speed;
        public float Angle;
        public float VisionFactor;
        public float VisionRadius;
        public float Radius;
        public BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Speed = velocity.Length();
            this.Angle = MathF.Atan2(velocity.Y, velocity.X);
            this.Radius = (float)texture.Width / 2; 
            this.VisionRadius = Radius * visionFactor;
        }
    }
}