using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


//using System.Numerics;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BoidManager
    {
        List<BoidEntity> _boids = new List<BoidEntity>();
        private Texture2D _boidTexture;

        public BoidManager(Texture2D texture)
        {
            _boidTexture = texture;
        }
        public void SpawnBoid()
        {
            Vector2 spawnPoint = Utils.RandomSpawnPosition();
            Vector2 spawnVel = Utils.InitialVelocity(Utils.InitialAngle(), Utils.InitialSpeed());
            BoidEntity newBoid = new BoidEntity(_boidTexture, spawnPoint, spawnVel);
            _boids.Add(newBoid);
        }
        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds*Constants.accFactor;
            Constants.BoundaryType boundaryType = Constants.tempCond;  
            foreach (BoidEntity b in _boids)
            {
                // Initializing movement vectors
                Vector2 sep = Vector2.Zero;
                Vector2 align = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                Vector2 steer = Vector2.Zero;
                Vector2 vecTor = Vector2.Zero;

                // Neighbour variables initilized
                b.neighbours.Clear();

                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;
                    switch (boundaryType)
                    {
                        case Constants.BoundaryType.Wrap:
                            vecTor = BoundaryCond.TorusDistance(b.position, other.position, Constants.SWidth, Constants.SHeight);
                            break;
                        case Constants.BoundaryType.Bounce:
                            vecTor = BoundaryCond.distVect(b.position, other.position);
                            break;
                        case Constants.BoundaryType.Steer:
                            vecTor = BoundaryCond.distVect(b.position, other.position);

                            // Distances to edges of screen
                            float left = b.position.X - b.boidRadius;
                            float right = Constants.SWidth - b.boidRadius - b.position.X;
                            float top = b.position.Y - b.boidRadius;
                            float bottom = Constants.SHeight - b.boidRadius - b.position.Y;
                            Vector2 boundSteer = BoundaryCond.steerBoid(left, right, top, bottom);

                            break;
                    }
                    float distSq = vecTor.Length();
                    float visSq = b.visionRadius;// * b.visionRadius;
                    if (distSq > visSq) continue;

                    align += other.velocity;
                    center += other.position;
                    float closeSq = visSq / Constants.visionFactor;
                    if (distSq < closeSq)
                    {
                        sep += -vecTor;
                    }
                    b.neighbours.Add(other);
                }
                if (b.neighbours.Count > 0)
                {
                    align /= b.neighbours.Count;
                    center /= b.neighbours.Count;
                    steer += (align - b.velocity) * Constants.alignFactor;
                    steer += (center - b.position) * Constants.coheFactor;
                    steer += sep * Constants.sepFactor;
                    steer += Utils.RandomVector(Constants.RandomSteer, Constants.RandomSteer);
                }
                b.velocity += steer * dt * Utils.RandomFloatRange(0,Constants.RandomVel);
                if (b.velocity.LengthSquared() > Constants.maxSpeed * Constants.maxSpeed)
                {
                    b.velocity = Vector2.Normalize(b.velocity) * Constants.maxSpeed;
                }

                if (b.velocity.LengthSquared() < Constants.minSpeed * Constants.minSpeed)
                {
                    b.velocity = Vector2.Normalize(b.velocity) * Constants.minSpeed;
                }

            }
            foreach (BoidEntity b in _boids)
            {
                switch (boundaryType)
                {
                    case Constants.BoundaryType.Wrap:
                        b.position += b.velocity * dt;
                        b.position = BoundaryCond.Wrap(b.position);
                        break;
                    case Constants.BoundaryType.Bounce:
                        b.position += b.velocity * dt;
                        b.velocity = BoundaryCond.bounce(b.velocity, b.position);
                        break;
                    case Constants.BoundaryType.Steer:

                        break;
                }
            }

        }
       
        public void Draw(SpriteBatch sb)
        {
            foreach (BoidEntity b in _boids)
            {
                sb.Draw(b.texture, b.position, Color.White);
            }
        }
    }
}