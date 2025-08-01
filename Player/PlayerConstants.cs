using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Boids
{
    public static class PlayerConstants
    {
        public static float eatRadiusFactor = 2f;
        public static float maxSpeed = 240f;
        public static float maxAccel = 100f;
        public static float drag = 100f;
        public static float maxTurn = MathF.PI * 0.95f;
        public static float sprintTime = 1f;
        public static float sprintAcc = 2f;
        public static float sprintSpeed = 2f;
        public static float sprintCoolDown = 5f;
    }
}