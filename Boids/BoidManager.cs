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
        private List<BoidEntity> _boids = new List<BoidEntity>();
        public IReadOnlyList<BoidEntity> listOfBoids => _boids;
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
        public void RemoveBoid()
        {
            if (_boids.Count > 0)
            {
                _boids.RemoveAt(_boids.Count - 1);
            }
        }
        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds * Constants.accFactor;
            foreach (BoidEntity b in _boids)
            {
                // Initializing movement vectors
                Vector2 sep = Vector2.Zero;
                Vector2 align = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                Vector2 steer = Vector2.Zero;
                Vector2 vecTor = Vector2.Zero;
                Vector2 boundSteer = Vector2.Zero;

                // Neighbour variables initilized
                b.neighbours.Clear();

                if (Constants.bcCondition == Constants.BoundaryType.Steer)
                {
                    // Initial position check
                    //b.Position = Utils.PosCheck(b.Position, b.boidRadius);
                    boundSteer = BoundaryCond.steerBoid(b.Position, b.boidRadius);
                    b.ResetThrottle();
                    if (boundSteer != Vector2.Zero)
                    {
                        // Determining the new angle
                        float desiredAngle = MathF.Atan2(boundSteer.Y, boundSteer.X);
                        float rawDelta = desiredAngle - b.angle;
                        float delta = MathHelper.WrapAngle(rawDelta);
                        float maxTurn = Constants.MaxTurnPerSec * dt;
                        float turn = MathHelper.Clamp(delta, -maxTurn, maxTurn);
                        b.angle += turn;

                        // Adding throttling when near wall
                        float turnIntensity = MathF.Min(MathF.Abs(turn), MathF.PI / 2f) / (MathF.PI / 2f);
                        b.Throttle = MathHelper.Lerp(Constants.speedDown, 1f, turnIntensity); // Has to between 0 and 1
                        b.Velocity = new Vector2(MathF.Cos(b.angle), MathF.Sin(b.angle)) * b.speed;
                    }
                }
                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;
                    switch (Constants.bcCondition)
                    {
                        case Constants.BoundaryType.Wrap:
                            vecTor = BoundaryCond.TorusDistance(b.Position, other.Position, Constants.SWidth, Constants.SHeight);
                            break;
                        case Constants.BoundaryType.Bounce:
                        case Constants.BoundaryType.Steer:
                            vecTor = BoundaryCond.distVect(b.Position, other.Position);
                            break;
                    }
                    float distLen = vecTor.Length();
                    float visLen = b.visionRadius;// * b.visionRadius;
                    if (distLen > visLen) continue;

                    align += other.Velocity;
                    center += other.Position;
                    float closeLen = visLen / Constants.visionFactor;
                    if (distLen < closeLen)
                    {
                        sep += -vecTor;  // This was -vecTor, but it kinda made sep bad....
                    }
                    b.neighbours.Add(other);
                }
                if (Constants.bcCondition == Constants.BoundaryType.Steer)
                {
                    steer += boundSteer * Constants.steerWeight;
                }
                if (b.neighbours.Count > 0)
                {
                    align /= b.neighbours.Count;
                    center /= b.neighbours.Count;
                    steer += (align - b.Velocity) * Constants.alignFactor;
                    steer += (center - b.Position) * Constants.coheFactor;
                    steer += sep * Constants.sepFactor;
                    steer += Utils.RandomVector(Constants.RandomSteer, Constants.RandomSteer);
                }
                b.Velocity += steer * dt * Utils.RandomFloatRange(0, Constants.RandomVel);
                if (b.Velocity.LengthSquared() > Constants.maxSpeed * Constants.maxSpeed)
                {
                    b.Velocity = Vector2.Normalize(b.Velocity) * Constants.maxSpeed * b.Throttle;
                }

                if (b.Velocity.LengthSquared() < Constants.minSpeed * Constants.minSpeed)
                {
                    b.Velocity = Vector2.Normalize(b.Velocity) * Constants.minSpeed * b.Throttle;
                }
            }
            foreach (BoidEntity b in _boids)
            {
                switch (Constants.bcCondition)
                {
                    case Constants.BoundaryType.Wrap:
                        b.Position += b.Velocity * dt;
                        b.Position = BoundaryCond.Wrap(b.Position);
                        break;
                    case Constants.BoundaryType.Bounce:
                        b.Position += b.Velocity * dt;
                        b.Velocity = BoundaryCond.bounce(b.Velocity, b.Position);
                        break;
                    case Constants.BoundaryType.Steer:
                        b.Position += b.Velocity * dt;
                        b.Position = BoundaryCond.PosCheck(b.Position, b.boidRadius);
                        break;
                }
            }

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