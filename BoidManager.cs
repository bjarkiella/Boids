using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
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

                if (boundaryType == Constants.BoundaryType.Steer)
                {
                    Vector2 boundSteer = BoundaryCond.steerBoid(b.position, b.boidRadius);
                    if (boundSteer != Vector2.Zero)
                    {
                        float desiredAngle = MathF.Atan2(boundSteer.Y, boundSteer.X);
                        float delta = desiredAngle - b.angle;
                        delta = (delta + MathF.PI) % (MathF.PI * 2) - MathF.PI;

                        float maxTurn = Constants.MaxTurnPerSec * dt;
                        float turn = MathHelper.Clamp(delta, -maxTurn, maxTurn);
                        b.angle += turn;

                        b.velocity = new Vector2(MathF.Cos(b.angle), MathF.Sin(b.angle)) * b.speed;    

                        //Vector2 newDir = Vector2.Normalize(b.velocity) + boundSteer * Constants.steerWeight;
                        //b.velocity = Vector2.Normalize(newDir) * b.speed;
                    }
                }

                // Move this section down and remove the speed etc.
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

                            break;
                    }
                    float distLen = vecTor.Length();
                    float visLen = b.visionRadius;// * b.visionRadius;
                    if (distLen > visLen) continue;

                    align += other.velocity;
                    center += other.position;
                    float closeLen = visLen / Constants.visionFactor;
                    if (distLen < closeLen)
                    {
                        sep += -vecTor;
                    }
                    b.neighbours.Add(other);
                }
                if (boundaryType == Constants.BoundaryType.Steer)
                {
                    Vector2 boundSteer = BoundaryCond.steerBoid(b.position, b.boidRadius);
                    steer += boundSteer * Constants.steerWeight;
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
                        b.position += b.velocity * dt ;
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