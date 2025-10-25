using System;

namespace Boids.Player
{
    public static class PlayerConstants
    {
        public const float eatRadiusFactor = 2f;
        public const float maxSpeed = 240f;
        public const float maxAccel = 500f;
        public const float drag = 5;
        public const float MaxTurn = MathF.PI * 0.95f;
        public const float sprintTime = 1f;
        public const float sprintAcc = 2f;
        public const float sprintSpeed = 2f;
        public const float sprintCoolDown = 5f;
        public const float wallProx = 0.03f;
        public const float cloudTransSpeed = 3.0f;
    }
}
