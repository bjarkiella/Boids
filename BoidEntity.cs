using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BoidEntity
    {
        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        private float _throttle = 1f;
        public float Throttle
        {
            get => _throttle;
            set => _throttle = MathF.Max(0f, MathF.Min(1f, value));
        }

        public float speed;
        public float angle;
        public readonly float boidRadius;
        public float visionRadius => boidRadius * Constants.visionFactor;
        public List<BoidEntity> neighbours;
        public BoidEntity(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.speed = velocity.Length();
            this.angle = MathF.Atan2(velocity.Y, velocity.X);
            this.boidRadius = (float)texture.Width / 2;
            this.neighbours = new List<BoidEntity>();
        }
        public void ResetThrottle()
        {
            Throttle = 1f;
        }


    }
}