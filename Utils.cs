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
        public const int SHeight = 720;
        public const int SWidth = 1280;
        public static float visionFactor = 2f;

        public static float RandomFloatRange(float a, float b)
        {
            Random random = new Random();
            return (float)random.NextDouble() * (MathF.Max(a, b) - MathF.Min(a, b)) + MathF.Min(a, b);
        }
        public static Vector2 RandomSpawnPosition()
        {
            return new Vector2(RandomFloatRange(0f, SWidth), RandomFloatRange(0f, SHeight));
        }
        public static float InitialSpeed()
        {
            return Utils.RandomFloatRange(0f, 1f); 
        }
        public static float InitialAngle()
        {
            return Utils.RandomFloatRange(0f, 2 * MathF.PI); 
        }
    }
}