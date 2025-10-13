using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.Shared;

namespace Boids.Player
{
    internal class PlayerEntity(
            Texture2D texture,
            Vector2 position,
            Vector2 velocity,
            float eatRadiusFactor):
        BaseEntity(texture, position, velocity, eatRadiusFactor)
    {
        private float _sprintTimeLeft = 0f;
        private float _coolDown = 0f;
        private bool _sprinting = false;
        private float _sprintAcc = 1f;
        private float _sprintSpeed = 1f;
        private BC.Edge? _edge;
        public float EatRadius => VisionRadius;
        public bool EatBoid { get; private set; } = false;
        public Rectangle PlayerBox => new (
                (int)(Position.X - Radius),
                (int)(Position.Y - Radius),
                (int)(Radius*2),
                (int)(Radius*2));

        internal void SteerTowards(Vector2 desiredDir, float maxTurnRate) => RotateTowardsDir(desiredDir,maxTurnRate); 

        internal void Integrate() 
        {
            Position += Velocity * Dt;
        }

        public void Update(KeyboardState current, KeyboardState _prevKeyboardState)
        {
            _edge = BC.ClosestEdge(Position, Radius,Radius*2,PlayerConstants.wallProx);

            // Keyboard inputs for player, edge is used to stop pushing beyond edge
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
            if (current.IsKeyDown(Keys.LeftShift) && !_prevKeyboardState.IsKeyDown(Keys.LeftShift) &&
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
                _sprintTimeLeft -= Dt;
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
                _coolDown -= Dt;
                _coolDown = MathF.Max(0f, _coolDown);
            }

            // Movement of player set 
            if (move != Vector2.Zero)
            {
                Vector2 heading = Vector2.Normalize(move);
                ApplyAccel(heading,PlayerConstants.maxAccel,_sprintAcc);
                UpdateVelocity(Speed,0f,PlayerConstants.maxSpeed,_sprintSpeed);
            }

            else
            {
                ApplyDrag(PlayerConstants.drag);
            }
            Integrate();

            Position = BC.PosCheck(Position, Radius);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, Color.White);
        }
    }
}
