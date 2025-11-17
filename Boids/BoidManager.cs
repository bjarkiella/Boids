using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Shared;
using Boids.Particles;

namespace Boids.Boids
{
    internal class BoidManager(BoidResources resources)
    {
        private readonly List<BoidEntity> _boids = []; 
        public IReadOnlyList<BoidEntity> ListOfBoids => _boids;

        private readonly BoidResources _resources = resources;
        private readonly Texture2D _boidTexture = resources.BoidAnimation.Texture;
        private readonly List<Rectangle> _frames = resources.BoidAnimation.Frames;
        private readonly float _frameDuration = resources.BoidAnimation.FrameDuration;
        private readonly ParticleManager _bloodParticles = new(resources.BloodParticleAnimation);

        protected static float Dt => Time.Delta;

        public void SpawnBoid()
        {
            Vector2 spawnPoint = Utils.RandomSpawnPosition();
            Vector2 spawnVel = Utils.InitialVelocity(Utils.RandomAngle(), Utils.RandomSpeed());
            int randomStart = Random.Shared.Next(0, _frames.Count);
            Animation boidAnimation = new (_boidTexture,_frames,_frameDuration,true,randomStart);
            BoidEntity newBoid = new (boidAnimation, _resources, spawnPoint, spawnVel, BoidConstants.visionFactor);
            _boids.Add(newBoid);
        }
        public void RemoveBoid()
        {
            if (_boids.Count > 0)
            {
                _boids.RemoveAt(_boids.Count - 1);
            }
        }
        
        public void Update(Vector2? eatPos= null, float? eatRadius = null, bool eatBoid=false, float opacity=1.0f)
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
                if (eatPos.HasValue && b.InVisionRange(eatPos.Value,opacity)){
                    b.Alert();
                    b.SteerFromPlayer(eatPos.Value);

                    if (eatBoid && (b.Position - eatPos.Value).Length() <= eatRadius.Value)
                    {
                        eatenBoid.Add(b);
                        Vector2 deathPos = b.DeathByCake();
                        _bloodParticles.SpawnParticles(deathPos, Vector2.Zero, lifetime: 1.0f, isGravity: true, rotation: 0f);
                    }
                    b.Animation?.Update();
                    if (!b.IsFleeing)
                        b.ApplyBC();
                    b.Integrate();
                    b.Update();
                    continue; //BOID DEAD OR ESCPAED, NEXT!
                } 
                foreach (BoidEntity other in _boids)
                {
                    // Some pre-checks
                    if (other == b) continue;
                    if (eatenBoid.Contains(other)) continue;
                    if (!b.InVisionRange(other.Position,opacity)) continue;
                    BoidFlocking.GatherNeighbours(ref align, ref center, ref sep, b, other);
                    neighbours++;
                }

                if (neighbours > 0)
                {
                    Vector2 nSteer = BoidFlocking.FlockSteer(neighbours,b,align,center,sep,steer);
                    b.UpdateSteerVelocity(nSteer);
                }

                b.Animation?.Update();
                b.Update();
                b.ApplyBC();
                b.Integrate();
            }
            
            // Update blood particles (they persist after boids die)
            _bloodParticles.Update();
            
            foreach (BoidEntity b in eatenBoid) _boids.Remove(b);
        }

        public void Draw(SpriteBatch sb)
        {
            _bloodParticles.Draw(sb);  // Draw blood particles first (behind boids)
            foreach (BoidEntity b in _boids)
            {
                b.Draw(sb);
            }
        }
    }
}
