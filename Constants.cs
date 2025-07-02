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
        public static float warnInPerc = 0.01f;
        public static float warnOutPerc = warnInPerc + 0.01f;
        public static float warnDistance = warnInPerc * MathF.Max(SWidth, SHeight);
        public static float WarnInX = warnDistance;
        public static float WarnInY = warnDistance;
        public const int roundNumber = 2;
        public const float boidMinFactor = 0f;
        public const float boidMaxFactor = 5f;
        public static float sepFactor = 1.8f;
        public static float alignFactor = 0.7f;
        public static float coheFactor = 0.7f;
        public const float accFactor = 2f;
        public const float maxSpeed = 120f;
        public const float minSpeed = 60f;
        public const float RandomSteer = 10f;
        public const float RandomVel = 1.1f;
        public const float steerWeight = 0.7f;
        public const float MaxTurnPerSec = MathF.PI * 0.75f;
        public const float speedDown = 0.5f;
        public const int PHeight = 100;
        public const int PWidth = SWidth;
        public enum BoundaryType
        {
            Wrap, Bounce, Steer
        }
        public const BoundaryType tempCond = BoundaryType.Steer;
    }
}