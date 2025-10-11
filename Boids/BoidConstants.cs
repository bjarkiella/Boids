using System;

namespace Boids.Boids
{
    public static class BoidConstants
{
        public const float visionFactor = 20f; // If this is below 20, then boids get stuck to walls, relats to wallProx and wallTurn 
        public const float boidMinFactor = 0f;
        public const float boidMaxFactor = 5f;
        public const float accFactor = 2f;
        public const float maxSpeed = 200f;
        public const float minSpeed = 100f;
        public const float RandomSteer = 10f;
        public const float RandomVel = 1.1f;
        public const float steerWeight = 0.7f;
        public const float MaxTurn = MathF.PI * 0.75f;
        public const float speedDown = 0.75f;
        public const float speedUp = 1.5f;
        public const float wallProx = 0.35f; // When the boid should start seeing the wall (0<wallprox<1)
        public const float wallTurn = 0.8f; // When the boid should start turning (higher = closer) (0<wallTurn<1)
        public const float boidAccel = 20.0f;
        public const int maxStuck = 5;
        public const float MaxWiggleAngle = 25f;
        public const float MaxWiggleSpeed = 5f;

        public static float SepFactor {get; set;} = 1.8f;
        public static float AlignFactor {get; set;} = 0.7f;
        public static float CoheFactor {get; set;} = 0.7f;
}
}
