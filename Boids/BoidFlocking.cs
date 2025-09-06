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
            if (d < closeLen) sep -= distance; // sep -= distance/d // sep -= distance/ (d*d);  
        }    
        internal static Vector2 FlockSteer(float neighbours, BoidEntity checkBoid)
        {
            Vector2 align = Vector2.Zero;
            Vector2 center = Vector2.Zero;
            Vector2 sep = Vector2.Zero;
            Vector2 steer = Vector2.Zero;

            align /= neighbours;
            center /= neighbours;
            steer += (align - checkBoid.Velocity) * BoidConstants.alignFactor;
            steer += (center - checkBoid.Position) * BoidConstants.coheFactor;
            steer += sep * BoidConstants.sepFactor;
            steer += Utils.RandomVector(BoidConstants.RandomSteer, BoidConstants.RandomSteer);
            return steer;   
        }

    }
}

