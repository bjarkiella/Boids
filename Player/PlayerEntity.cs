using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// TODO: Change this class so it makes sense the player uses it
namespace Boids
{
    internal class PlayerEntity:BaseEntity
    {
        private Vector2 _heading = Vector2.Zero;
        private float _speed = 0f;

        public PlayerEntity(Texture2D texture, Vector2 position, Vector2 velocity, float visionFactor) : base(texture, position, velocity, visionFactor)
        {

        }
        public void Update(GameTime gameTime, KeyboardState current, KeyboardState _prevKeyboardState)
        {
            float dt = Utils.deltaTime(gameTime) ;
            bool edge = BoundaryCond.EdgeCheck(Position, Radius);

            // Keyboard inputs for player
            // TODO: Add checks if im at the edge or not, flesh out BoundCond class
            Vector2 move = Vector2.Zero;
            if (current.IsKeyDown(Keys.Up))
            {
                move.Y -= 1;
            }
            if (current.IsKeyDown(Keys.Down))
            {
                move.Y += 1;
            }
            if (current.IsKeyDown(Keys.Right))
            {
                move.X += 1;
            }
            if (current.IsKeyDown(Keys.Left))
            {
                move.X -= 1;
            }
            if (current.IsKeyDown(Keys.Space) && !_prevKeyboardState.IsKeyDown(Keys.Space))
            {


            }

            if (move != Vector2.Zero)
            {
                // Smoothing out the heading (same as in steer in boidmanager)
                Vector2 heading = Vector2.Normalize(move);
                float currentAngle = MathF.Atan2(_heading.Y, _heading.X);
                float desiredAngle = MathF.Atan2(heading.Y, heading.X);
                float rawDelta = desiredAngle - currentAngle;
                float angleDiff = MathHelper.WrapAngle(rawDelta); 
                float maxTurn = PlayerConstants.maxTurn * dt;
                float turn = MathHelper.Clamp(angleDiff, -maxTurn, maxTurn);
                float newAngle = currentAngle + turn; 
                _heading = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));

                // Smoothing out the speed
                _speed += PlayerConstants.maxAccel * dt;
                _speed = MathF.Min(_speed, PlayerConstants.maxSpeed);
            }

            else if (MathF.Round(Velocity.Length()) < 1e-3 || edge)
            {
                _speed = 0f;
            }
            else
            {
                //Velocity *= 1f - PlayerConstants.drag * dt;
                _speed -= PlayerConstants.drag * dt;
                _speed = MathF.Max(_speed, 0f);
            }
            Velocity = _heading * _speed;
            Position += Velocity * dt;
            Position = BoundaryCond.PosCheck(Position, Radius);
            Console.WriteLine(_speed);
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(this.Texture, this.Position, Color.White);
        }
    }
}