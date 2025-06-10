using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BoidManager
    {
        List<BoidEntity> _boids = new List<BoidEntity>();
        private Texture2D _boidTexture;

        public BoidManager(Texture2D texture)
        {
            _boidTexture = texture;
        }
        public void SpawnBoid()
        {
            BoidEntity newBoid = new BoidEntity(_boidTexture, Utils.RandomSpawnPosition(), Utils.InitialSpeed(), Utils.InitialAngle());
            _boids.Add(newBoid);
        }
        public void Update(GameTime gt)
        {
            Vector2 direction;
            float steerAngle;
            float left, right, top, bottom, dt, throttle,desired,maxTurn,turnIntensity;
            foreach (BoidEntity b in _boids)
            {
                dt = (float)gt.ElapsedGameTime.TotalSeconds;
                // Checking collision with with walls (bounce effect)
                // b.angle = bounce(b.angle,b.position);
                
                // Calculating the distance to the closest edge
                left = b.position.X - b.boidRadius;
                right = Constants.SWidth - b.boidRadius - b.position.X;
                top = b.position.Y - b.boidRadius;
                bottom = Constants.SHeight - b.boidRadius - b.position.Y;

                steerAngle = steerBoid(left, right, top, bottom);

                throttle = 1f;
                if (steerAngle != 0f)
                {
                    desired = b.angle + steerAngle;
                    maxTurn = Constants.MaxTurnPerSec * dt;
                    b.angle += MathHelper.Clamp(desired, -maxTurn, maxTurn);
                    turnIntensity = MathF.Min(MathF.Abs(desired), MathF.PI / 2) / (MathF.PI / 2);
                    throttle = MathHelper.Lerp(1f, Constants.speedDown, turnIntensity);
                }

                //b.angle = MathHelper.Lerp(b.angle, b.angle + steerAngle, Constants.steerStrength * dt);

                direction = new Vector2(MathF.Cos(b.angle), MathF.Sin(b.angle));
                b.position += direction * b.speed * dt*throttle;
                
                //b.angle = MathF.Atan2(direction.Y, direction.X);
            }
        }
        private float bounce(float angle, Vector2 position)
        {
            // Checking collision with with walls (bounce effect)
            if (position.Y <= 0 || position.Y >= Constants.SHeight)
            {
                angle *= -1;
            }
            if (position.X <= 0 || position.X >= Constants.SWidth)
            {
                angle = MathF.PI - angle;
            }
            return angle;
        }
       
        private float steerBoid(float left, float right, float top, float bottom)
        {
            float x=0f, y = 0f;
            if (left < Constants.WarnInX)
            {
                x += (left - Constants.WarnInX) / Constants.WarnInX;
            }
            else if (right < Constants.WarnInX)
            {
                x -= (Constants.WarnInX - right) / Constants.WarnInX;
            }
            if (top < Constants.WarnInY)
            {
                y += (top - Constants.WarnInY) / Constants.WarnInY;
            }
            else if (bottom < Constants.WarnInY)
            {
                y += (Constants.WarnInY - bottom) / Constants.WarnInY;
            }

            if (x == 0f && y == 0f) return 0f;

            Vector2 steer = Vector2.Normalize(new Vector2(x, y));
            return MathF.Atan2(steer.Y, steer.X);  
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (BoidEntity b in _boids)
            {
                sb.Draw(b.texture, b.position, Color.White);
            }
        }
    }
}