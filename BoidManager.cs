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
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            foreach (BoidEntity b in _boids)
            {
                // Reset per-boid accumulators
                Vector2 sep    = Vector2.Zero;
                Vector2 align  = Vector2.Zero;
                Vector2 center = Vector2.Zero;
                int neighbourCount = 0;

                b.neighbours.Clear();

                foreach (BoidEntity other in _boids)
                {
                    if (other == b) continue;

                    // Torus-wrapped distance
                    (float distSq, Vector2 away) = TorusDistanceSq(b.position, other.position,Constants.SWidth, Constants.SHeight);

                    float visSq = b.visionRadius * b.visionRadius;
                    if (distSq > visSq) continue;        // not a neighbour

                    //  In-range neighbour 
                    b.neighbours.Add(other);
                    neighbourCount++;

                    // separation (inverse-square weighting)
                    sep += Vector2.Normalize(away) / distSq;

                    // alignment (use neighbour’s forward vector)
                    align += new Vector2(MathF.Cos(other.angle), MathF.Sin(other.angle));

                    // cohesion (accumulate positions)
                    center += other.position;
                }

                if (neighbourCount > 0)
                {
                    center /= neighbourCount;                               // average position
                    align = Vector2.Normalize(align / neighbourCount);     // average heading
                }

                // Normalise & weight each steering component
                if (sep != Vector2.Zero) sep = Vector2.Normalize(sep) * Constants.sepFactor;
                if (align != Vector2.Zero) align = align * Constants.alignFactor;
                Vector2 cohe = Vector2.Zero;
                if (neighbourCount > 0)
                    cohe = Vector2.Normalize(center - b.position) * Constants.coheFactor;

                // Current forward vector
                Vector2 forward = new Vector2(MathF.Cos(b.angle), MathF.Sin(b.angle));

                // Desired direction
                Vector2 desired = Vector2.Normalize(forward + sep + align + cohe);

                // Integrate position & angle
                b.position += desired * b.speed * dt;
                b.position = Wrap(b.position, b.boidRadius);
                b.angle = MathF.Atan2(desired.Y, desired.X);   // optional: rotate sprite
            }
        }

        public static (float distSq, Vector2 away) TorusDistanceSq(Vector2 me, Vector2 other, float width, float height)
        {
            float dx = me.X - other.X;
            dx -= MathF.Round(dx / width)  * width;   // shortest X diff

            float dy = me.Y - other.Y;
            dy -= MathF.Round(dy / height) * height;  // shortest Y diff

            return (dx * dx + dy * dy, new Vector2(dx, dy));      
        }
        private static Vector2 Wrap(Vector2 pos, float r)
        {
            float spanX = Constants.SWidth + 2 * r;
            float spanY = Constants.SHeight + 2 * r;
            pos.X = (pos.X + r + spanX) % spanX - r;
            pos.Y = (pos.Y + r + spanY) % spanY - r;
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