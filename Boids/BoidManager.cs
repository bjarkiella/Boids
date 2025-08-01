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
                Vector2 pBoidTor = Vector2.Zero;
                if (eatPos == null)
                {
                    eatPos = Vector2.Zero;
                }

                // Neighbour variables initilized
                b.Neighbours.Clear();

                // Checking if player is close
                bool playerClose = false;
                if (eatPos.HasValue)
                {
                    pBoidTor = distanceVector(b.Position,eatPos.Value);
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
                    // Make a new angle and a turn, similar to what is done with steer and give it a little speed booooozt    
                        
                }

                // Need to check here two things:
                // 1. Is there a player nearby, that is the _player.Position
                // 2. Is the player trying to eat.
                // If the player is nearby, then avoid
                // And if he is trying to eat and is within the eating distance, then remove
                // So all in all, the update function needs three inputs, position, eatradius and if trying to eat
                // Then it could be a logic like, if only position has value, then I dont need eatradius or the other one etc.

                if (BoidConstants.bcCondition == BoidConstants.BoundaryType.Steer)
                {
                    // Initial position check
                    boundSteer = BoidBC.steerBoid(b.Position, b.Radius);
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

                        // Applying throttling when near wall
                        float turnIntensity = MathF.Min(MathF.Abs(turn), MathF.PI / 2f) / (MathF.PI / 2f);
                        b.Throttle = MathHelper.Lerp(BoidConstants.speedDown, 1f, turnIntensity); // Has to between 0 and 1
                        b.Velocity = new Vector2(MathF.Cos(b.Angle), MathF.Sin(b.Angle)) * b.Speed;
                    }
                }
                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;
                    vecTor = distanceVector(b.Position,other.Position);
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
                        b.Position = BoidBC.Wrap(b.Position);
                        break;
                    case BoidConstants.BoundaryType.Bounce:
                        b.Position += b.Velocity * dt;
                        b.Velocity = BoidBC.bounce(b.Velocity, b.Position);
                        break;
                    case BoidConstants.BoundaryType.Steer:
                        b.Position += b.Velocity * dt;
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