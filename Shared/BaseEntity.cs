using System;
using Boids.Boids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Shared
//TODO: I propably need to move some of the boid specifi things to the boidentity, like for example integrate and stuff like that
{
    internal class BaseEntity 
    {
        private Vector2 _velocity;
        private Vector2 _fallbackHeading = new(1,0);

        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { 
            get => _velocity; 
            set{
                _velocity = value;
                if (_velocity.LengthSquared() > Constants.ZeroCompare) _fallbackHeading = Vector2.Normalize(Velocity);
            } 
        } // Used to check if entity is at, or close to zero speed 
        public float VisionFactor { get; set; }

        // Cap margins  
        private readonly float visionCap = MathF.Min(Constants.ActiveWidth,Constants.ActiveHeight)/2f; 
    

        protected static float Dt => Time.Delta;
        protected float Angle => MathF.Atan2(Velocity.Y, Velocity.X);
        protected float Speed => Velocity.Length();
        protected float Radius => (float)Texture.Width /2;
        protected float VisionRadius => MathF.Min(Radius * VisionFactor,visionCap);
        protected Vector2 Heading => Speed > Constants.ZeroCompare ? Vector2.Normalize(Velocity) : _fallbackHeading;
        protected Vector2 RandomHeading => Speed > Constants.ZeroCompare ? Vector2.Normalize(Velocity + Utils.RandomVector(-0.05f,0.05f)) : _fallbackHeading;
        

        public BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            VisionFactor = visionFactor;
        }
    
        public void RotateTowardsDir(Vector2 desiredDir, float maxRate)
        {
            if (desiredDir.LengthSquared() <= Constants.ZeroCompare) return;

            float desiredAngle = MathF.Atan2(desiredDir.Y, desiredDir.X);
            float delta = MathHelper.WrapAngle(desiredAngle - Angle);
            float maxTurn = maxRate * Dt;
            float turn = MathHelper.Clamp(delta, -maxTurn, maxTurn);
            float newangle = turn + Angle;
            Vector2 dir = Utils.NewDirection(newangle);
            _fallbackHeading = dir;
            Velocity = dir * Speed;
        }

        public float ApplyAccTo(float targetSpeed, float accel)
        {
            if (targetSpeed <= Speed) return targetSpeed;
                    
            float currentSpeed;
            float nextSpeed = Speed + accel*Dt;
            if (targetSpeed > Speed) currentSpeed = nextSpeed;  
            else currentSpeed = nextSpeed - accel*Dt;
            return currentSpeed;
            // Velocity = Heading * currentSpeed;
        }
        
        public void ApplyAccel(Vector2 desiredDir, float maxAccel, float accel) 
        {
            Velocity += desiredDir * (maxAccel * accel) * Dt; 

        }
        public void ApplyDrag(float dragFactor)
        {
            float k = MathF.Max(0f, 1f - dragFactor * Dt);
            Velocity *= k;
        }

        public void UpdateVelocity(float initSpeed, float minSpeed,float maxSpeed, float sFactor)
        {
            float clampSpeed = MathHelper.Clamp(initSpeed,minSpeed,maxSpeed) * sFactor;
            if (MathF.Abs(Speed - clampSpeed) > 1e-6f) Velocity = clampSpeed * Heading;
        }

        // public void CornerCheck()
        // {
        //     if (Position.LengthSquared() == _prevPosition.LengthSquared())  // Is the boid stuck in a corner
        //     {
        //         Console.WriteLine("I think I might be stuck");
        //         _stuckframe++;
        //     }
        //     else _stuckframe = 0;
        //
        //     if (_stuckframe >= BoidConstants.maxStuck)  // The boid is stuck and starts accelerating from corner
        //     {
        //         Console.WriteLine("Trying to get looooose");
        //         _targetSpeed = Utils.RandomSpeed();
        //         _escapeDir = Utils.NewDirection(Utils.RandomAngle());
        //         _accelFromCorner = true;
        //         _stuckframe = 0;
        //     }
        //     if (_stuckframe == 0 && _accelFromCorner)  //The boid is accelerating from the corner
        //     {
        //         float currentSpeed = ApplyAccTo(_targetSpeed,BoidConstants.boidAccel);
        //
        //         Console.WriteLine("Im trying to accelerate from the corner: "+currentSpeed);
        //         if (_targetSpeed <= currentSpeed) 
        //         {
        //             _accelFromCorner = false;
        //             _escapeDir = Vector2.Zero;
        //         }
        //         else Velocity = currentSpeed * _escapeDir;
        //     }
        // }
        // public void Integrate()
        // {
        //     _prevPosition = Position;
        //     Position += Velocity * Dt;
        //
        //             BC.Edge? edge = BC.ClosestEdge(Position,Radius,VisionRadius,BoidConstants.wallProx);
        //             if (edge!= null) SteerFromEdge(edge);
        //     Position = BC.PosCheck(Position,Radius); 
        //     CornerCheck();
        // }
    }
}
