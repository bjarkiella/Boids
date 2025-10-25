using System;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Boids
{
    internal class BoidEntity(
            Animation animation,
            Vector2 position,
            Vector2 velocity,
            float visionFactor)
        :BaseEntity(animation.Texture, position,velocity, visionFactor,animation)
    {
        private bool _inTurn = false;
        private float _preSpeed = 0f;
        private bool _accel = false;

        private int _stuckframe =0;
        private Vector2 _prevPosition;
        private float _targetSpeed;
        private bool _accelFromCorner = false;
        private Vector2 _escapeDir;
        private bool _fleeing = false;
        internal bool IsFleeing => _fleeing;
        internal bool IsEscapingCorner => _accelFromCorner;

        public float SpeedFactor = 1f;

        public readonly Animation _animation = animation;

        internal float BoidVisionRadius() => VisionRadius;
        internal float CloseVision() => BoidVisionRadius()/BoidConstants.visionFactor;
        internal void ResetSpeedFactor() => SpeedFactor = 1f;
        internal void SteerFromEdge(BC.Edge? edge, float speedFactor = BoidConstants.speedDown)
        {
            if (edge == null)
            {
                throw new InvalidOperationException("The input 'edge' should never be null");
            }
            Vector2 steerDir = BoidBC.SteerBoid(Position,Radius,BoidVisionRadius(),BoidConstants.wallTurn);
            RotateTowardsDir(steerDir,BoidConstants.MaxTurn);
            SpeedFactor = BoidConstants.speedDown;
            UpdateVelocity(_preSpeed,BoidConstants.minSpeed,BoidConstants.maxSpeed,speedFactor); 
            Position = BC.PosCheck(Position,Radius);
        }
        internal void SteerFromPlayer(Vector2 playerPos)
        {
            _fleeing = true;
            Vector2 avoidVector = Position - playerPos;
            RotateTowardsDir(avoidVector,BoidConstants.MaxTurn);
            SpeedFactor = BoidConstants.speedUp;
            UpdateVelocity(Speed,BoidConstants.minSpeed,BoidConstants.maxSpeed,SpeedFactor);
            ResetSpeedFactor();
        }
        internal float WiggleSpeed()
        {
            float wiggleSpeed = Utils.RandomFloatRange(-BoidConstants.MaxWiggleSpeed,BoidConstants.MaxWiggleSpeed);
            return Speed + wiggleSpeed;
        }
        internal Vector2 WiggleHeading()
        {
            float wiggleAngle = Utils.RandomFloatRange(-Utils.DegToRad(BoidConstants.MaxWiggleAngle),Utils.DegToRad(BoidConstants.MaxWiggleAngle));
            float newAngle = Angle + wiggleAngle;

            Vector2 newHeading = Utils.NewDirection(newAngle); 
            Vector2 smoothHeading = Vector2.Lerp(Heading,newHeading,0.12f);
            return Vector2.Normalize(smoothHeading);
        }

        internal void UpdateSteerVelocity(Vector2 steer)
        {
            Velocity += steer * Dt * Utils.RandomFloatRange(0, BoidConstants.RandomVel);

        }
        internal void CornerCheck()
        {
            if (Position.LengthSquared() == _prevPosition.LengthSquared())  // Is the boid stuck in a corner
            {
                // Console.WriteLine("I think I might be stuck");
                _stuckframe++;
            }
            else _stuckframe = 0;

            if (_stuckframe >= BoidConstants.maxStuck)  // The boid is stuck and starts accelerating from corner
            {
                // Console.WriteLine("Trying to get looooose");
                _targetSpeed = Utils.RandomSpeed();
                _escapeDir = Utils.NewDirection(Utils.RandomAngle());
                _accelFromCorner = true;
                _stuckframe = 0;
            }
            // if (_stuckframe == 0 && _accelFromCorner)  //The boid is accelerating from the corner
            if (_accelFromCorner)
            {
                float currentSpeed = ApplyAccTo(_targetSpeed,BoidConstants.boidAccel);

                // Console.WriteLine("Im trying to accelerate from the corner: "+currentSpeed);
                if (_targetSpeed <= currentSpeed) 
                {
                    _accelFromCorner = false;
                    _escapeDir = Vector2.Zero;
                }
                else Velocity = currentSpeed * _escapeDir;
            }
        }
        internal void Integrate()
        {
            _prevPosition = Position;
            Position += Velocity * Dt;

            float edgeProx = _fleeing ? 0.99f : 0.95f;

            // float edgeProx = 0.95f; 
            BC.Edge? edge = BC.ClosestEdge(Position,Radius,BoidVisionRadius(),edgeProx);
            if (edge!= null)
            {
                float speedFactor = _fleeing ? BoidConstants.speedUp : BoidConstants.speedDown;
                SteerFromEdge(edge,speedFactor);
            }
            else 
            {
                if (!_fleeing)
                {
                    float wiggleSpeed = WiggleSpeed();
                    Vector2 wiggleHeading = WiggleHeading();
                    Velocity = wiggleHeading * wiggleSpeed;
                }
            }
            _fleeing = false;
            CornerCheck();
        }
        internal void ApplyBC()
        {
            BC.Edge? edge = BC.ClosestEdge(Position,Radius,BoidVisionRadius(),BoidConstants.wallProx);
            if (edge == null && !_inTurn && !_accel) return; // Here there is nothing happening 
            else if (edge == null && _inTurn && !_accel) // Here the boid stops turning and starts accelerating
            {
                // Console.WriteLine("Ive stopped turning");
                _inTurn = false;
                _accel = true;
                ResetSpeedFactor();

                return;
            }
            else if (edge == null && _accel)  // Here the boid is accelerating
            {
                float currentSpeed = ApplyAccTo(_preSpeed,BoidConstants.boidAccel);
                if (_preSpeed <= currentSpeed) _accel = false;
                else Velocity = currentSpeed * Heading;
                // Console.WriteLine("My current speed is: " + Velocity.Length());
            }
            else if (edge != null && !_inTurn)  // Here the boid starts turning
            {
                // Console.WriteLine("Ive started turning now");
                _preSpeed = Speed;  // check if max speed is exceeded?
                _inTurn = true;
                SteerFromEdge(edge);
            }
            else if (edge!= null && _inTurn && !_accel) SteerFromEdge(edge);  // Here the boid is currently turning

        }

        internal bool InVisionRange(Vector2 pos,float opacityFactor) {
        float effVisionFactor = BoidVisionRadius() * opacityFactor;
            float distaSq = Vector2.DistanceSquared(Position,pos);
            float visionSq = effVisionFactor * effVisionFactor;
            return distaSq <= visionSq;
        }

        internal void Draw(SpriteBatch sb)
        {
            float scale = 2f;
            Vector2 origin = new (_animation.FrameWidth/2f, _animation.FrameHeight/2f);
            if (Velocity.X >= 0)
                sb.Draw(_animation.Texture, Position, _animation.CurrentFrame, Color.White, 0f, origin, scale , SpriteEffects.None,0f);
            else if (Velocity.X < 0)
                sb.Draw(_animation.Texture, Position, _animation.CurrentFrame, Color.White, 0f, origin, scale , SpriteEffects.FlipHorizontally,0f);
        }

    }
}
