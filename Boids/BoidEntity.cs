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
        public float SpeedFactor = 1f;
        public List<BoidEntity> Neighbours; 

        public BoidEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor): base(texture, position,velocity, visionFactor)
        {
            Neighbours = new List<BoidEntity>();
        }
        public void ResetSpeedFactor()
        {
            SpeedFactor = 1f;
        }
    }
}