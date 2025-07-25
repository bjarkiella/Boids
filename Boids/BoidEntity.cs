using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BoidEntity:BaseEntity
    {
        private float _throttle = 1f;
        public float Throttle
        {
            get => _throttle;
            set => _throttle = MathF.Max(0f, MathF.Min(1f, value));
        }
        public List<BoidEntity> Neighbours; 

        public BoidEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor): base(texture, position,velocity, visionFactor)
        {
            Neighbours = new List<BoidEntity>();
        }
        public void ResetThrottle()
        {
            Throttle = 1f;
        }
    }
}