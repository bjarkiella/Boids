using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Boids
{
    public static class Utils
    {

        public static float RandomFloatRange(float a, float b)
        {
            Random random = new Random();
            return (float)random.NextDouble() * (MathF.Max(a, b) - MathF.Min(a, b)) + MathF.Min(a, b);
        }
        public static Vector2 RandomSpawnPosition()
        {
            float wCons = 1f - Constants.warnOutPerc;
            return new Vector2(RandomFloatRange(Constants.SWidth*Constants.warnOutPerc, Constants.SWidth*wCons),
            RandomFloatRange(Constants.SHeight*Constants.warnOutPerc, Constants.SHeight*wCons));
        }
       
        public static float distPoints(Vector2 vector1, Vector2 vector2)
        {
            float distance;
            distance = MathF.Sqrt(MathF.Pow(vector2.X - vector1.X, 2) + MathF.Pow(vector2.Y - vector1.Y, 2));
            return distance;
        }
        public static float InitialSpeed()
        {
            return RandomFloatRange(1f * 120, 2f * 120);
        }
        public static float InitialAngle()
        {
            return RandomFloatRange(0f, 2 * MathF.PI); 
        }
    }
}