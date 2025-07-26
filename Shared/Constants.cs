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
        public static float warnInPerc = 0.01f;
        public static float warnOutPerc = warnInPerc + 0.01f;
        public static float warnDistance = warnInPerc * MathF.Max(SWidth, SHeight);
        public static float WarnInX = warnDistance;
        public static float WarnInY = warnDistance;
        public const int roundNumber = 2;
        public const int ActiveHeight = SHeight - PHeight;
        public const int ActiveWidth = SWidth;
        public const int PHeight = 150;
        public const int PWidth = SWidth;
    }
}