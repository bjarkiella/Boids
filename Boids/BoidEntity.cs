using System;
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

        internal float CloseVision() => VisionRadius/BoidConstants.visionFactor;

        internal bool EdgeCloseCheck() => BC.CloseToEdge(Position,Radius,VisionRadius);

        internal void SteerTowards(Vector2 desiredDir, float maxTurnRate) => RotateTowardsDir(desiredDir,maxTurnRate); 

        internal static Vector2 SteerFromEdgeDir(BC.Edge? edge)
        {
            return edge switch
            {
                BC.Edge.Left => new Vector2(+1f,0f),
                BC.Edge.Right => new Vector2(-1f,0f),
                BC.Edge.Top => new Vector2(0f,+1f),
                BC.Edge.Bottom => new Vector2(0f,-1f),
                _ => Vector2.Zero
            };
        }
        internal void SteerFromEdge()
        {
            BC.Edge? edge = BC.EdgeCheck(Position, Radius); 
            if (!EdgeCloseCheck())
            {
                throw new InvalidOperationException
                    ("ERROR: The boid is not next to an edge, calling SteerFromEdge() not required");
            }
            if (edge == null)
            {
                throw new InvalidOperationException
                    ("ERROR: Unable to determine which edge boid is next to");
            }
            Vector2 steerDir = SteerFromEdgeDir(edge);
            SpeedFactor = BoidConstants.speedDown;
            UpdateVelocity(BoidConstants.minSpeed,BoidConstants.maxSpeed,SpeedFactor); 
            SteerTowards(steerDir,BoidConstants.MaxTurn);
            ResetSpeedFactor();

        }
        internal void SteerFromPlayer(Vector2 playerPos)
        {
            Vector2 avoidVector = Position - playerPos;
            SteerTowards(avoidVector,BoidConstants.MaxTurn);
            SpeedFactor = BoidConstants.speedUp;
            UpdateVelocity(BoidConstants.minSpeed,BoidConstants.maxSpeed,SpeedFactor);
            ResetSpeedFactor();
        }

        internal void UpdateSteerVelocity(Vector2 steer)
        {
            Velocity += steer * Dt * Utils.RandomFloatRange(0, BoidConstants.RandomVel);

        }
        internal void ApplyBC(Constants.BoundaryType bType)
        {
            if (!EdgeCloseCheck()) return;
            switch (bType)
            {
                case Constants.BoundaryType.Steer:
                    SteerFromEdge();
                    Position = BC.PosCheck(Position,Radius);
                    break;

                case Constants.BoundaryType.Bounce:
                    Velocity = BoidBC.Bounce(Velocity, Position);
                    break;

                case Constants.BoundaryType.Wrap:
                    Position = BoidBC.Wrap(Position);
                    break;

                default:
                    break;
            }
        }

        internal bool InVisionRange(Vector2 pos) {
            float distaSq = Vector2.DistanceSquared(Position,pos);
            float visionSq = VisionFactor * VisionFactor;
            if (distaSq <= visionSq) return true;
            return false;
        }
        // Put boundary conditions here

        public void ResetSpeedFactor()
            => SpeedFactor = 1f;

    }
}
