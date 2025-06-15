using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    public static class BoundaryCond
    {
        public static Vector2 TorusDistance(Vector2 a, Vector2 b, float width, float height)
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
        }
        
        public static Vector2 Wrap(Vector2 pos)
        {
            if (pos.X >= Constants.SWidth) pos.X -= Constants.SWidth;
            if (pos.X <= 0) pos.X += Constants.SWidth;
            if (pos.Y >= Constants.SHeight) pos.Y -= Constants.SHeight;
            if (pos.Y <= 0) pos.Y += Constants.SHeight;

            return pos;
        }

        public static Vector2 bounce(Vector2 velocity, Vector2 position)
        {
            // Checking collision with with walls (bounce effect)
            if (position.Y <= 0 || position.Y >= Constants.SHeight)
            {
                velocity.Y *= -1;
            }
            if (position.X <= 0 || position.X >= Constants.SWidth)
            {
                velocity.X *= -1;
            }
            return velocity;
        }
        
        public static Vector2 steerBoid(float left, float right, float top, float bottom)
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

            if (x == 0f && y == 0f) return Vector2.Zero;

            return Vector2.Normalize(new Vector2(x, y));
        }
        public static Vector2 distVect(Vector2 a, Vector2 b)
        {
            return b - a;
        }
    }
}