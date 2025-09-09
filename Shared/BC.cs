using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Boids.Shared
{
    public static class BC
    {
        public enum Edge {Left, Right,Top,Bottom}
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
        
        public static float[] PosEdge(Vector2 position, float radius)
        {
            float[] edgeList;
            edgeList = new float[4];
            edgeList[(int) Edge.Left] = position.X - radius;
            edgeList[(int) Edge.Right] = Constants.ActiveWidth - radius - position.X;
            edgeList[(int) Edge.Top] = position.Y - radius;
            edgeList[(int) Edge.Bottom] = Constants.ActiveHeight - radius - position.Y;
            return edgeList;
        }

        public static Edge? ClosestEdge(Vector2 position, float radius, float proxRadius, float proxTrigger=0.03f)
        {
            float[] edgeList = PosEdge(position,radius);
            float minDist = Utils.MinValueArray(edgeList);
            int idx = Array.IndexOf(edgeList,minDist);
            Edge closest = (Edge)idx;

            if (proxTrigger <= 0f) return closest;
            else if (proxTrigger < MathHelper.Clamp(1f-(minDist/proxRadius),0f,1f))
                return closest;
            else
                return null;



            // if (position.X-radius/4 <= 0)
            // {
            //     return Edge.Left;
            // }
            // else if (position.X + radius*2 >= Constants.ActiveWidth)
            // {
            //     return Edge.Right;
            // }
            // else if (position.Y - radius/4 <= 0)
            // {
            //     return Edge.Top; 
            // }
            // else if (position.Y + radius >= Constants.ActiveHeight)
            // {
            //     return Edge.Bottom; 
            // }
            // return null;
        }


        public static Vector2 PosCheck(Vector2 position, float radius)
        {
            if (position.X-radius/4 <= 0)
            {
                position.X = radius/4;
            }
            else if (position.X + radius*2 >= Constants.ActiveWidth)
            {
                position.X = Constants.ActiveWidth - radius*2;        
            }
            if (position.Y - radius/4 <= 0)
            {
                position.Y = radius/4;
            }
            else if (position.Y + radius >= Constants.ActiveHeight)
            {
                position.Y = Constants.ActiveHeight - radius;
            }
            return position;
        }
        public static Vector2 distVect(Vector2 a, Vector2 b)
        {
            return b - a;
        }
    }
}
