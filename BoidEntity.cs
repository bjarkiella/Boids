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
        public float speed;
        public float angle;
        public readonly float boidRadius;
        public float visionRadius => boidRadius * Constants.visionFactor;
        public BoidEntity(Texture2D texture, Vector2 position, float speed, float angle)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            this.angle = angle;
            this.boidRadius = (float)texture.Width/2;
        }


    }
}