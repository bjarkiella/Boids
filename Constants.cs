using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boids
{
    public static class Constants
    {

        public const int SHeight = 720*2;
        public const int SWidth = 1280*2;
        public static float visionFactor = 5f;
        public static float steerStrength = 2f;
        public static float warnInPerc = 0.15f;
        public static float warnOutPerc = warnInPerc + 0.01f;
        public static float speedDown = 0.5f;
        public static float WarnInX = SWidth * warnInPerc;
        public static float WarnInY = SHeight * warnInPerc;
        public static float WarnOutX = SWidth * warnOutPerc;
        public static float WarnOutY = SHeight * warnOutPerc;
        public const float MaxTurnPerSec = MathF.PI * 0.75f;
        public const float sepFactor = 1.3f;
        public const float alignFactor = 1.2f;
        public const float coheFactor = 1.8f;
    }
}