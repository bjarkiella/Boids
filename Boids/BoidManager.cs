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
using Boids.Shared;

namespace Boids.Boids
{
    internal class BoidManager
    {
        private List<BoidEntity> _boids = new List<BoidEntity>();
        public IReadOnlyList<BoidEntity> listOfBoids => _boids;
        private Texture2D _boidTexture;
        protected static float Dt => Time.Delta;
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
        private Vector2 distanceVector(Vector2 first, Vector2 second)
        {
            Vector2 distVector = Vector2.Zero;
            switch (BoidConstants.bcCondition)
            {
                case BoidConstants.BoundaryType.Wrap:
                    distVector = BC.TorusDistance(first, second, Constants.ActiveWidth, Constants.ActiveHeight);
                    break;
                case BoidConstants.BoundaryType.Bounce:
                case BoidConstants.BoundaryType.Steer:
                    distVector = BC.distVect(first, second);
                    break;
            }
            return distVector;
        }
        public void Update(GameTime gt, Vector2? eatPos= null, float? eatRadius = null, bool eatBoid=false)
        {
            List<BoidEntity> eatenBoid = new List<BoidEntity>();

            foreach (BoidEntity b in _boids)
            {
                // Initializing movement vectors
                Vector2 sep = Vector2.Zero;
                Vector2 align = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                Vector2 steer = Vector2.Zero;
                Vector2 vecTor = Vector2.Zero;
                Vector2 boundSteer = Vector2.Zero;
                Vector2 pBoidTor = Vector2.Zero;

                // Neighbour variables initilized
                b.Neighbours.Clear();

                // Checking if player is close
                bool playerClose = false;
                if (eatPos.HasValue)
                {
                    pBoidTor = distanceVector(b.Position, eatPos.Value);
                    if (pBoidTor.Length() > b.VisionRadius)
                    {
                        playerClose = false;
                    }
                    else
                    {
                        playerClose = true;
                        Console.WriteLine("OMG HE IS CLOSE");
                    }
                }
                // If close evasive manuovers!
                if (playerClose)
                {
                    Vector2 avoidVector = b.Position - eatPos.Value;
                    b.SteerTowards(avoidVector,BoidConstants.MaxTurnPerSec);
                    // 
                    // float turn = Utils.calcTurnAngle(avoidVector, dt, b.Angle, BoidConstants.MaxTurnPerSec);
                    // b.Angle += turn;
                    b.SpeedFactor = BoidConstants.speedUp;
                    // b.Velocity = new Vector2(MathF.Cos(b.Angle), MathF.Sin(b.Angle)) * b.Speed;
                    b.ClampSpeed(BoidConstants.minSpeed,BoidConstants.maxSpeed,b.SpeedFactor);
                    // if (b.Velocity.LengthSquared() > BoidConstants.maxSpeed * BoidConstants.maxSpeed)
                    // {
                    //     b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.maxSpeed * b.SpeedFactor;
                    // }
                    //
                    // if (b.Velocity.LengthSquared() < BoidConstants.minSpeed * BoidConstants.minSpeed)
                    // {
                    //     b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.minSpeed * b.SpeedFactor;
                    // }
                    b.ResetSpeedFactor();
                    if (eatBoid && avoidVector.Length() <= eatRadius.Value)
                    {
                        eatenBoid.Add(b);
                    }
                    continue;
                }

                if (BoidConstants.bcCondition == BoidConstants.BoundaryType.Steer)
                {
                    // Initial position check
                    boundSteer = BoidBC.steerBoid(b.Position, b.Radius);
                    b.ResetSpeedFactor();
                    if (boundSteer != Vector2.Zero)
                    {
                        b.SteerTowards(boundSteer,BoidConstants.MaxTurnPerSec);
                        // Determining the new angle
                        // float turn = Utils.calcTurnAngle(boundSteer, dt, b.Angle, BoidConstants.MaxTurnPerSec);
                        // b.Angle += turn;

                        // Applying throttling when near wall
                        float turnIntensity = MathF.Min(MathF.Abs(turn), MathF.PI / 2f) / (MathF.PI / 2f);
                        b.SpeedFactor = MathHelper.Lerp(BoidConstants.speedDown, 1f, turnIntensity); // Has to between 0 and 1
                        b.Velocity = new Vector2(MathF.Cos(b.Angle), MathF.Sin(b.Angle)) * b.Speed;
                    }
                }
                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;
                    vecTor = distanceVector(b.Position, other.Position);
                    if (vecTor.Length() > b.VisionRadius) continue;

                    align += other.Velocity;
                    center += other.Position;
                    float closeLen = b.VisionRadius / BoidConstants.visionFactor;
                    if (vecTor.Length() < closeLen)
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
                b.Velocity += steer * Dt * Utils.RandomFloatRange(0, BoidConstants.RandomVel);
                // if (b.Velocity.LengthSquared() > BoidConstants.maxSpeed * BoidConstants.maxSpeed)
                // {
                    // b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.maxSpeed * b.SpeedFactor;
                    b.ClampSpeed(BoidConstants.minSpeed,BoidConstants.maxSpeed,b.SpeedFactor);
                // }

                // if (b.Velocity.LengthSquared() < BoidConstants.minSpeed * BoidConstants.minSpeed)
                // {
                    // b.Velocity = Vector2.Normalize(b.Velocity) * BoidConstants.minSpeed * b.SpeedFactor;
                // }
            }
            foreach (BoidEntity b in eatenBoid) _boids.Remove(b);
            
            foreach (BoidEntity b in _boids)
            {
                switch (BoidConstants.bcCondition)
                {
                    case BoidConstants.BoundaryType.Wrap:
                        b.Integrate();
                        b.Position = BoidBC.Wrap(b.Position);
                        break;
                    case BoidConstants.BoundaryType.Bounce:
                        b.Integrate();
                        b.Velocity = BoidBC.bounce(b.Velocity, b.Position);
                        break;
                    case BoidConstants.BoundaryType.Steer:
                        b.Integrate();
                        b.Position = BC.PosCheck(b.Position, b.Radius);
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
