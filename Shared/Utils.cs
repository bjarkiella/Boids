using System;
using Microsoft.Xna.Framework;

namespace Boids.Shared
{
    public static class Utils
    {
        static Random random = new (); 
        public static float RandomFloatRange(float a, float b)
        {
            random = new Random();
            return (float)random.NextDouble() * (MathF.Max(a, b) - MathF.Min(a, b)) + MathF.Min(a, b);
        }
        public static Vector2 RandomVector(float a, float b)
        {
            return new Vector2(RandomFloatRange(a, b), RandomFloatRange(a, b));
        }

        public static Vector2 RandomSpawnPosition()
        {
            float wCons = 1f - Constants.warnOutPerc;
            return new Vector2(RandomFloatRange(Constants.ActiveWidth*Constants.warnOutPerc, Constants.ActiveWidth*wCons),
            RandomFloatRange(Constants.ActiveHeight*Constants.warnOutPerc, Constants.ActiveHeight*wCons));
        }

        public static Vector2 NewDirection(float radAngle)
        {
            return new Vector2(MathF.Cos(radAngle),MathF.Sin(radAngle));
        }
        public static Vector2 InitialVelocity(float spawnAngle, float spawnSpeed)
        {
            return new Vector2(MathF.Cos(spawnAngle),MathF.Sin(spawnAngle)) * spawnSpeed; 
        }
        public static float InitialSpeed()
        {
            return RandomFloatRange(1f * 120, 2f * 120);
        }
        public static float InitialAngle()
        {
            return RandomFloatRange(0f, 2 * MathF.PI); 
        }
        public static float MinValueArray(float[] arrayList)
        {
            float minDist = float.MaxValue;
            for (int i = 0; i < arrayList.Length; i++)
                if (arrayList[i] < minDist) minDist = arrayList[i];
            return minDist;
        }
        
 
    }
}
