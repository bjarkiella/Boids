using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

//using System.Numerics;
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
            Vector2 spawnPoint = Utils.RandomSpawnPosition();
            Vector2 spawnVel = Utils.InitialVelocity(Utils.InitialAngle(), Utils.InitialSpeed());
            BoidEntity newBoid = new BoidEntity(_boidTexture, spawnPoint, spawnVel);
            _boids.Add(newBoid);
        }
        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds*Constants.accFactor;

            foreach (BoidEntity b in _boids)
            {
                // Initializing movement vectors
                Vector2 sep = Vector2.Zero;
                Vector2 align = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                Vector2 steer = Vector2.Zero;
                Vector2 vecTor = Vector2.Zero;

                // Neighbour variables initilized
                b.neighbours.Clear();

                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;

                    vecTor = TorusDistanceSq(b.position, other.position, Constants.SWidth, Constants.SHeight);
                    //float distSq = vecTor.LengthSquared();
                    float distSq = vecTor.Length();
                    float visSq = b.visionRadius;// * b.visionRadius;
                    if (distSq > visSq) continue;

                    align += other.velocity;
                    center += other.position;
                    float closeSq = visSq / Constants.visionFactor;
                    if (distSq < closeSq)
                    {
                        // sep += b.position - other.position;
                        sep += -vecTor;
                    }
                    b.neighbours.Add(other);
                }
                if (b.neighbours.Count > 0)
                {
                    align /= b.neighbours.Count;
                    center /= b.neighbours.Count;
                    steer += (align - b.velocity) * Constants.alignFactor;
                    steer += (center - b.position) * Constants.coheFactor;
                    steer += sep * Constants.sepFactor;
                    steer += Utils.RandomVector(Constants.RandomSteer, Constants.RandomSteer);
                }
                b.velocity += steer * dt * Utils.RandomFloatRange(0,Constants.RandomVel);
                if (b.velocity.LengthSquared() > Constants.maxSpeed * Constants.maxSpeed)
                {
                    b.velocity = Vector2.Normalize(b.velocity) * Constants.maxSpeed;
                }

                if (b.velocity.LengthSquared() < Constants.minSpeed * Constants.minSpeed)
                {
                    b.velocity = Vector2.Normalize(b.velocity) * Constants.minSpeed;
                }

            }
            foreach (BoidEntity b in _boids)
            {
                b.position += b.velocity * dt;
                b.position = Wrap(b.position, b.boidRadius);
            }
        }
        public static Vector2 TorusDistanceSq(Vector2 a, Vector2 b, float width, float height)
        {
            float dx = MathF.Abs(b.X - a.X);
            float dy = MathF.Abs(b.Y - a.Y);

            if (dx > width / 2)
            {
                dx = width - dx;
            }
            if (dy > height / 2)
            {
                dy = height - dy;
            }

            return new Vector2(dx, dy);



            //float dx = b.X - other.X;
            //dx -= MathF.Round(dx / width) * width;   // shortest X diff

           // float dy = b.Y - other.Y;
            //dy -= MathF.Round(dy / height) * height;  // shortest Y diff

            //return dx * dx + dy * dy;
           // return new Vector2(dx, dy);  
            //dreturn new Vector2(dx, dy);
        }
        private static Vector2 Wrap(Vector2 pos, float r)
        {
            if (pos.X >= Constants.SWidth) pos.X -= Constants.SWidth;
            if (pos.X <= 0) pos.X += Constants.SWidth;
            if (pos.Y >= Constants.SHeight) pos.Y -= Constants.SHeight;
            if (pos.Y <= 0) pos.Y += Constants.SHeight;

            return pos;
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