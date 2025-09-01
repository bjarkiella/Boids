using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.Shared;

// TODO: Change this class so it makes sense the player uses it
namespace Boids.Player
{
    internal class PlayerEntity : BaseEntity
    {
        private Vector2 _heading = Vector2.Zero;
        private float _speed = 0f;
        private float _sprintTimeLeft = 0f;
        private float _coolDown = 0f;
        private bool _sprinting = false;
        private float _sprintAcc = 1f;
        private float _sprintSpeed = 1f;
        private BC.Edge? _edge;
        public float _eatRadius => VisionRadius;
        public bool EatBoid { get; private set; } = false;
        public PlayerEntity(Texture2D texture, Vector2 position, Vector2 velocity, float eatRadiusFactor) : base(texture, position, velocity, eatRadiusFactor)
        {

        }

        internal void SteerTowards(Vector2 desiredDir, float maxTurnRate) 
            => RotateTowardsDir(desiredDir,maxTurnRate); 

        internal void Clamp(float sFactor) 
            => UpdateVelocity(0f,PlayerConstants.maxSpeed,sFactor); 

        public void Update(GameTime gameTime, KeyboardState current, KeyboardState _prevKeyboardState)
        {
            float dt = Utils.deltaTime(gameTime);
            _edge = BC.EdgeCheck(Position, Radius);

            // Keyboard inputs for player
            Vector2 move = Vector2.Zero;
            if (current.IsKeyDown(Keys.Up) && _edge != BC.Edge.Top)
            {
                move.Y -= 1;
            }
            if (current.IsKeyDown(Keys.Down) && _edge != BC.Edge.Bottom)
            {
                move.Y += 1;
            }
            if (current.IsKeyDown(Keys.Right) && _edge != BC.Edge.Right) 
            {
                move.X += 1;
            }
            if (current.IsKeyDown(Keys.Left) && _edge != BC.Edge.Left) 
            {
                move.X -= 1;
            }
            if ((current.IsKeyDown(Keys.LeftShift) && !_prevKeyboardState.IsKeyDown(Keys.LeftShift)) &&
            !_sprinting &&
            _coolDown <= 0f &&
            move.Length() > 0)
            {
                _sprinting = true;
                _sprintTimeLeft = PlayerConstants.sprintTime;
                _sprintAcc = PlayerConstants.sprintAcc;
                _sprintSpeed = PlayerConstants.sprintSpeed;
            }
            if (current.IsKeyDown(Keys.Space) && _prevKeyboardState.IsKeyDown(Keys.Space))
            {
                EatBoid = true;
            }
            else
            {
                EatBoid = false;
            }

            // Sprinting conditions 
            if (_sprinting)
            {
                _sprintTimeLeft -= dt;
                _sprintTimeLeft = MathF.Max(0f, _sprintTimeLeft);
                if (_sprintTimeLeft <= 0f)
                {
               _sprinting = false;
                    _sprintTimeLeft = 0f;
                    _sprintAcc = 1f;
                    _sprintSpeed = 1f;
                    _coolDown = PlayerConstants.sprintCoolDown;
                }
            }
            else if (_coolDown >= 0f)
            {
                _coolDown -= dt;
                _coolDown = MathF.Max(0f, _coolDown);
            }

            // Movement of player set 
            if (move != Vector2.Zero)
            {
                // Smoothing out the heading (same as in steer in boidmanager)
                Vector2 heading = Vector2.Normalize(move);
                float currentAngle = MathF.Atan2(_heading.Y, _heading.X);
                float turn = Utils.calcTurnAngle(heading, dt, currentAngle, PlayerConstants.maxTurn);
                float newAngle = currentAngle + turn;
                _heading = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));

                // Smoothing out the speed
                _speed += PlayerConstants.maxAccel * _sprintAcc * dt;
                _speed = MathF.Min(_speed, PlayerConstants.maxSpeed) * _sprintSpeed;
                
                // Refactor stuff
                // RotateTowardsDir(move,PlayerConstants.MaxTurn);
                
                // b.SteerTowards(avoidVector,BoidConstants.MaxTurn);
                // 
                // float turn = Utils.calcTurnAngle(avoidVector, dt, b.Angle, BoidConstants.MaxTurnPerSec);
                // b.Angle += turn;
                // b.SpeedFactor = BoidConstants.speedUp;
                // b.Velocity = new Vector2(MathF.Cos(b.Angle), MathF.Sin(b.Angle)) * b.Speed;
                // b.ClampSpeed(BoidConstants.minSpeed,BoidConstants.maxSpeed,b.SpeedFactor);
            }

            else if (MathF.Round(Velocity.Length()) < 1e-3 || _edge != null)
            {
                _speed = 0f;
            }
            else
            {
                _speed -= PlayerConstants.drag * dt;
                _speed = MathF.Max(_speed, 0f);
            }
            Velocity = _heading * _speed;
            Position += Velocity * dt;
            Position = BC.PosCheck(Position, Radius);

        }
        private void sprint()
        {

        }
        private void moveFrame()
        {

        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(this.Texture, this.Position, Color.White);
        }
    }
}
