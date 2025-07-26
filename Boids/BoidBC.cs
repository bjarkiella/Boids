using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    public static class BoidBC
    {
        public static Vector2 Wrap(Vector2 pos)
        {
            if (pos.X >= Constants.ActiveWidth) pos.X -= Constants.ActiveWidth;
            if (pos.X <= 0) pos.X += Constants.ActiveWidth;
            if (pos.Y >= Constants.ActiveHeight) pos.Y -= Constants.ActiveHeight;
            if (pos.Y <= 0) pos.Y += Constants.ActiveHeight;

            return pos;
        }

        public static Vector2 bounce(Vector2 velocity, Vector2 position)
        {
            // Checking collision with with walls (bounce effect)
            if (position.Y <= 0 || position.Y >= Constants.ActiveHeight)
            {
                velocity.Y *= -1;
            }
            if (position.X <= 0 || position.X >= Constants.ActiveWidth)
            {
                velocity.X *= -1;
            }
            return velocity;
        }
        
        public static Vector2 steerBoid(Vector2 position, float radius)
        {
            // Calculating distance to edges    
            float left = position.X - radius;
            float right = Constants.ActiveWidth - radius - position.X;
            float top = position.Y - radius;
            float bottom = Constants.ActiveHeight - radius - position.Y;

            float x=0f, y = 0f;
            if (left < Constants.WarnInX)
            {
                x -= (left - Constants.WarnInX) / Constants.WarnInX;
            }
            if (right < Constants.WarnInX)
            {
                x -= (Constants.WarnInX - right) / Constants.WarnInX;
            }
            if (top < Constants.WarnInY)
            {
                y -= (top - Constants.WarnInY) / Constants.WarnInY;
            }
            if (bottom < Constants.WarnInY)
            {
                y -= (Constants.WarnInY - bottom) / Constants.WarnInY;
            }

            if (x == 0f && y == 0f) return Vector2.Zero;

            return Vector2.Normalize(new Vector2(x, y));
        }
    }
}