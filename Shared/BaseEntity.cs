using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Shared
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
        public Animation? Animation { get; }

        // Cap margins  
        private readonly float visionCap = MathF.Min(Constants.ActiveWidth,Constants.ActiveHeight)/2f; 
    

        protected static float Dt => Time.Delta;
        protected float Angle => MathF.Atan2(Velocity.Y, Velocity.X);
        protected float Speed => Velocity.Length();
        protected float Radius => Animation != null
            ? MathF.Max((float)Animation.FrameWidth/2,(float)Animation.FrameHeight/2)
            : (float)Texture.Width/2;

        protected float VisionRadius => MathF.Min(Radius * VisionFactor,visionCap);
        protected Vector2 Heading => Speed > Constants.ZeroCompare ? Vector2.Normalize(Velocity) : _fallbackHeading;
        protected Vector2 RandomHeading => Speed > Constants.ZeroCompare ? Vector2.Normalize(Velocity + Utils.RandomVector(-0.05f,0.05f)) : _fallbackHeading;
        

        public BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor, Animation animation)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            VisionFactor = visionFactor;
            Animation = animation;
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
    }
}
