using System;
using System.Collections.Generic;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.Shared;
using Boids.Boids;

namespace Boids.Player
{
    internal class PlayerEntity(
            Animation animation,
            Vector2 position,
            Vector2 velocity,
            float eatRadiusFactor):
        BaseEntity(animation.Texture, position, velocity, eatRadiusFactor,animation)
    {
        private float _sprintTimeLeft = 0f;
        private float _coolDown = 0f;
        private bool _sprinting = false;
        private float _sprintAcc = 1f;
        private float _sprintSpeed = 1f;
        private BC.Edge? _edge;
        private readonly Animation _animation = animation;
        private enum DirFace {Right,Left}
        private DirFace currentFace = DirFace.Right;

        private float _opacity = 1.0f; 

        private bool _showDebugCircle = false;

        public float EatRadius => VisionRadius;
        public float OpacityFactor => _opacity;
        public bool EatBoid { get; private set; } = false;
        public Rectangle PlayerBox => new (
                (int)(Position.X - Radius),
                (int)(Position.Y - Radius),
                (int)(Radius*2),
                (int)(Radius*2));

        private float _textureScale = 2f;



        internal void SteerTowards(Vector2 desiredDir, float maxTurnRate) => RotateTowardsDir(desiredDir,maxTurnRate); 

        internal void Integrate() 
        {
            Position += Velocity * Dt;
        }

        public void Update(KeyboardState current, KeyboardState _prevKeyboardState, List<Rectangle> intersectEntity)
        {
            _animation.Update();
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
                currentFace = DirFace.Right;
                move.X += 1;
            }
            if (current.IsKeyDown(Keys.Left) && _edge != BC.Edge.Left) 
            {
                currentFace = DirFace.Left;
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
            if (current.IsKeyDown(Keys.RightShift) && _prevKeyboardState.IsKeyDown(Keys.RightShift)) 
            {
               _showDebugCircle = !_showDebugCircle; 
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

            // Checking if clouds intersect player
            if (intersectEntity != null)
            {
                bool playerInCloud = EntityIntersection(intersectEntity);
                UpdateOpacity(playerInCloud);
            }
        }

        public bool EntityIntersection(List<Rectangle> intersectEntity)
        {
            bool playerTouch = false;
            Rectangle playerBox = PlayerBox;

            foreach (Rectangle intersectRect in intersectEntity)
            {
                if (playerBox.Intersects(intersectRect))
                {
                    playerTouch = true;
                    break;
                }
            }
            return playerTouch;
        }

        public void UpdateOpacity(bool isInCloud)
        {
            // Smoothly transition opacity
            float targetOpacity = isInCloud? 0.4f: 1.0f; 
            float transitionSpeed = PlayerConstants.cloudTransSpeed;

            if (_opacity < targetOpacity)
                _opacity = Math.Min(_opacity + transitionSpeed * Dt, targetOpacity);
            else if (_opacity > targetOpacity)
                _opacity = Math.Max(_opacity - transitionSpeed * Dt, targetOpacity);
        }
        public void Draw(SpriteBatch sb)
        {
            Color drawColor = Color.White * _opacity; 
            Vector2 origin = new (_animation.FrameWidth/2f, _animation.FrameHeight/2f);
            if (_showDebugCircle)
                sb.DrawCircle(Position,Game1.BoidVisionRadius * _opacity, 32, Color.Red *0.3f,2f);
            if (currentFace == DirFace.Right)
                sb.Draw(_animation.Texture, Position,_animation.CurrentFrame, drawColor,0f,origin ,_textureScale,SpriteEffects.None, 0f);
            else if (currentFace == DirFace.Left)
                sb.Draw(_animation.Texture, Position,_animation.CurrentFrame, drawColor, 0f,origin ,_textureScale,SpriteEffects.FlipHorizontally, 0f);
            else {
                throw new InvalidOperationException ("The Player direction face could not be determined");
            }
        }
    }
}
