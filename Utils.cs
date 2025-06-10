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
        public static float InitialSpeed()
        {
            return Utils.RandomFloatRange(1f*120, 2f*120); 
        }
        public static float InitialAngle()
        {
            return Utils.RandomFloatRange(0f, 2 * MathF.PI); 
        }
    }
}