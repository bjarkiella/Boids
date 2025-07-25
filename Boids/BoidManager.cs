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
            BoidEntity newBoid = new BoidEntity(_boidTexture, spawnPoint, spawnVel, BoidConstants.visionFactor);
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
            float dt = Utils.deltaTime(gt) * BoidConstants.accFactor;
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
                b.Neighbours.Clear();

                if (BoidConstants.bcCondition == BoidConstants.BoundaryType.Steer)
                {
                    // Initial position check
                    boundSteer = BoundaryCond.steerBoid(b.Position, b.Radius);
                    b.ResetThrottle();
                    if (boundSteer != Vector2.Zero)
                    {
                        // Determining the new angle
                        float desiredAngle = MathF.Atan2(boundSteer.Y, boundSteer.X);
                        float rawDelta = desiredAngle - b.Angle;
                        float delta = MathHelper.WrapAngle(rawDelta);
                        float maxTurn = BoidConstants.MaxTurnPerSec * dt;
                        float turn = MathHelper.Clamp(delta, -maxTurn, maxTurn);
                        b.Angle += turn;

                        // Adding throttling when near wall
                        float turnIntensity = MathF.Min(MathF.Abs(turn), MathF.PI / 2f) / (MathF.PI / 2f);
                        b.Throttle = MathHelper.Lerp(BoidConstants.speedDown, 1f, turnIntensity); // Has to between 0 and 1
                        b.Velocity = new Vector2(MathF.Cos(b.Angle), MathF.Sin(b.Angle)) * b.Speed;
                    }
                }
                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;
                    switch (BoidConstants.bcCondition)
                    {
                        case BoidConstants.BoundaryType.Wrap:
                            vecTor = BoundaryCond.TorusDistance(b.Position, other.Position, Constants.SWidth, Constants.SHeight);
                            break;
                        case BoidConstants.BoundaryType.Bounce:
                        case BoidConstants.BoundaryType.Steer:
                            vecTor = BoundaryCond.distVect(b.Position, other.Position);
                            break;
                    }
                    float distLen = vecTor.Length();
                    float visLen = b.VisionRadius;// * b.VisionRadius;
                    if (distLen > visLen) continue;

                    align += other.Velocity;
                    center += other.Position;
                    float closeLen = visLen / BoidConstants.visionFactor;
                    if (distLen < closeLen)
                    {
                        sep += -vecTor;  // This was -vecTor, but it kinda made sep bad....
                    }
                    b.Neighbours.Add(other);
                }
                if (BoidConstants.bcCondition == BoidConstants.BoundaryType.Steer)
                {
                    steer += boundSteer * BoidConstants.steerWeight;
                }
                if (b.Neighbours.Count > 0)
                {
                    align /= b.Neighbours.Count;
                    center /= b.Neighbours.Count;
                    steer += (align - b.Velocity) * BoidConstants.alignFactor;
                    steer += (center - b.Position) * BoidConstants.coheFactor;
                    steer += sep * BoidConstants.sepFactor;
                    steer += Utils.RandomVector(BoidConstants.RandomSteer, BoidConstants.RandomSteer);
                }
                b.Velocity += steer * dt * Utils.RandomFloatRange(0, BoidConstants.RandomVel);
                if (b.Velocity.LengthSquared() > BoidConstants.maxSpeed * BoidConstants.maxSpeed)
                {
                    b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.maxSpeed * b.Throttle;
                }

                if (b.Velocity.LengthSquared() < BoidConstants.minSpeed * BoidConstants.minSpeed)
                {
                    b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.minSpeed * b.Throttle;
                }
            }
            foreach (BoidEntity b in _boids)
            {
                switch (BoidConstants.bcCondition)
                {
                    case BoidConstants.BoundaryType.Wrap:
                        b.Position += b.Velocity * dt;
                        b.Position = BoundaryCond.Wrap(b.Position);
                        break;
                    case BoidConstants.BoundaryType.Bounce:
                        b.Position += b.Velocity * dt;
                        b.Velocity = BoundaryCond.bounce(b.Velocity, b.Position);
                        break;
                    case BoidConstants.BoundaryType.Steer:
                        b.Position += b.Velocity * dt;
                        b.Position = BoundaryCond.PosCheck(b.Position, b.Radius);
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