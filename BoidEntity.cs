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
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        public float speed;
        public float angle;
        public readonly float boidRadius;
        public float visionRadius => boidRadius * Constants.visionFactor;
        public List<BoidEntity> neighbours;
        public BoidEntity(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.speed = velocity.Length();
            this.angle = MathF.Atan2(velocity.Y,velocity.X);
            this.boidRadius = (float)texture.Width / 2;
            this.neighbours = new List<BoidEntity>();
        }


    }
}