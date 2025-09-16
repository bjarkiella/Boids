using Microsoft.Xna.Framework;
using Boids.Shared;

namespace Boids.Boids
{
    internal static class BoidFlocking
    {
        internal static void GatherNeighbours(ref Vector2 align, ref Vector2 center, ref Vector2 sep, BoidEntity checkBoid, BoidEntity otherBoid) {

            align += otherBoid.Velocity;
            center += otherBoid.Position;

            float closeLen = checkBoid.CloseVision(); 
            Vector2 distance = otherBoid.Position - checkBoid.Position;
            float d = distance.Length();
            if (d > Constants.ZeroCompare && d < closeLen) sep -=distance/(d*d);
                // sep += Vector2.Normalize(-distance) / (d*d) ; // sep -= distance/d // sep -= distance/ (d*d);  
        }    
        internal static Vector2 FlockSteer(float neighbours, BoidEntity checkBoid, Vector2 align, Vector2 center, Vector2 sep, Vector2 steer)
        {
            align /= neighbours;
            center /= neighbours;

            Vector2 v = checkBoid.Velocity;
            Vector2 p = checkBoid.Position;
            float maxSpeed = BoidConstants.maxSpeed;

            // ALIGNMENT: desired = normalize(avgVel) * maxSpeed, steer = desired - v  (tiny change)
            if (align.LengthSquared() > Constants.ZeroCompare)
            {
                Vector2 desiredAlign = Vector2.Normalize(align) * maxSpeed;
                steer += (desiredAlign - v) * BoidConstants.alignFactor;
            }

            // COHESION: seek COM => desired = normalize(COM - pos) * maxSpeed, steer = desired - v  (tiny change)
            Vector2 toCOM = center - p;
            if (toCOM.LengthSquared() > Constants.ZeroCompare)
            {
                Vector2 desiredCoh = Vector2.Normalize(toCOM) * maxSpeed;
                steer += (desiredCoh - v) * BoidConstants.coheFactor;
            }

            // SEPARATION: use accumulated repulsion; normalize late, desired - v  (tiny change)
            if (sep.LengthSquared() > Constants.ZeroCompare)
            {
                Vector2 desiredSep = Vector2.Normalize(sep) * maxSpeed;
                steer += (desiredSep - v) * BoidConstants.sepFactor;
            }

            // keep your little bit of randomness (unchanged)
            steer += Utils.RandomVector(BoidConstants.RandomSteer, BoidConstants.RandomSteer);

            return steer;

            // steer += (align - checkBoid.Velocity) * BoidConstants.alignFactor;
            // steer += (center - checkBoid.Position) * BoidConstants.coheFactor;
            // steer += sep * BoidConstants.sepFactor;
            // steer += Utils.RandomVector(BoidConstants.RandomSteer, BoidConstants.RandomSteer);
            // return steer;   
        }

    }
}

