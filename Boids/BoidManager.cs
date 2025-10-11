using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Shared;

namespace Boids.Boids
{
    internal class BoidManager(Texture2D texture)
    {
        private readonly List<BoidEntity> _boids = []; 
        public IReadOnlyList<BoidEntity> ListOfBoids => _boids;
        private readonly Texture2D _boidTexture = texture;
        protected static float Dt => Time.Delta;

        public void SpawnBoid()
        {
            Vector2 spawnPoint = Utils.RandomSpawnPosition();
            Vector2 spawnVel = Utils.InitialVelocity(Utils.RandomAngle(), Utils.RandomSpeed());
            BoidEntity newBoid = new (_boidTexture, spawnPoint, spawnVel, BoidConstants.visionFactor);
            _boids.Add(newBoid);
        }
        public void RemoveBoid()
        {
            if (_boids.Count > 0)
            {
                _boids.RemoveAt(_boids.Count - 1);
            }
        }
        
        public void Update(Vector2? eatPos= null, float? eatRadius = null, bool eatBoid=false)
        {
            List<BoidEntity> eatenBoid = [];

            foreach (BoidEntity b in _boids)
            {
                // Initializing movement vectors
                Vector2 sep = Vector2.Zero;
                Vector2 align = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                Vector2 steer = Vector2.Zero;

                // Neighbour variables initilized
                float neighbours = 0;

                // Checking if player is close
                if (eatPos.HasValue && b.InVisionRange(eatPos.Value)){
                    b.SteerFromPlayer(eatPos.Value);

                    if (eatBoid && (b.Position - eatPos.Value).Length() <= eatRadius.Value)
                    {
                        eatenBoid.Add(b);
                    }
                    b.ApplyBC(Constants.bcCondition);
                    b.Integrate();
                    continue; //BOID DEAD OR ESCPAED, NEXT!
                } 
                foreach (BoidEntity other in _boids)
                {
                    // Some pre-checks
                    if (other == b) continue;
                    if (eatenBoid.Contains(other)) continue;
                    if (!b.InVisionRange(other.Position)) continue;
                    BoidFlocking.GatherNeighbours(ref align, ref center, ref sep, b, other);
                    neighbours++;
                }

                if (neighbours > 0)
                {
                    Vector2 nSteer = BoidFlocking.FlockSteer(neighbours,b,align,center,sep,steer);
                    b.UpdateSteerVelocity(nSteer);
                }

                b.ApplyBC(Constants.bcCondition);
                b.Integrate();
            }
            foreach (BoidEntity b in eatenBoid) _boids.Remove(b);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (BoidEntity b in _boids)
            {
                sb.Draw(b.Texture, b.Position, Color.White);
            }
        }
    }
}
