using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boids
{
    public static class Constants
    {

        public const int SHeight = 720 * 1;
        public const int SWidth = 1280 * 1;
        public static float visionFactor = 20f;
        public static float warnInPerc = 0.05f;
        public static float warnOutPerc = 0.05f;
        public static float WarnInX = SWidth * warnInPerc;
        public static float WarnInY = SHeight * warnInPerc;
        public const float sepFactor = 1.8f;
        public const float alignFactor = 0.7f;
        public const float coheFactor = 0.7f;
        public const float accFactor = 2f;
        public const float maxSpeed = 120f;
        public const float minSpeed = 60f;
        public const float RandomSteer = 10f;
        public const float RandomVel = 1.1f;
    }
}