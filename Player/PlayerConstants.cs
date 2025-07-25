using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Boids
{
    public static class PlayerConstants
{
        public static float visionFactor = 1f;
        public static float maxSpeed = 240f;
        public static float maxAccel = 100f;
        public static float drag = 2f;
        public static float maxTurn = MathF.PI * 0.95f;
    }
}