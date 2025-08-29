using System.Collections.Generic;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Boids
{
    internal class BoidEntity(
            Texture2D texture,
            Vector2 position,
            Vector2 velocity,
            float visionFactor)
        :BaseEntity(texture, position,velocity, visionFactor)
    {
        public float SpeedFactor = 1f;
        public List<BoidEntity> Neighbours { get; } = [];
        internal void SteerTowards(Vector2 desiredDir, float maxTurnRate) 
            => RotateTowardsDir(desiredDir,Dt,maxTurnRate); 

        public void ResetSpeedFactor()
            => SpeedFactor = 1f;

    }
}
