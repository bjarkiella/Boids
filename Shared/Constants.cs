using System;
using Microsoft.Xna.Framework;

namespace Boids.Shared
{
    public static class Constants
{
        public static int SHeight {get; set;} = 720 * 1;
        public static int SWidth {get; set;} = 1280 * 1;
        public const int DefaultPanelHeight = 200;
        public static int PHeight {get; set;} = DefaultPanelHeight;
        public static int PWidth {get; set;}= SWidth;
        public static int ActiveHeight => SHeight - PHeight;
        public static int ActiveWidth => SWidth;
        public static Vector2 CameraPosition {get; set;} = Vector2.Zero;
        public static Vector2 ScreenSize => new (ActiveWidth,ActiveHeight);

        public const float warnInPerc = 0.01f;
        public const float warnOutPerc = warnInPerc + 0.01f;
        public static float warnDistance = warnInPerc * MathF.Max(SWidth, SHeight);
        public static float WarnInX = warnDistance;
        public static float WarnInY = warnDistance;
        public const float ZeroCompare = 1e-10f;   
        public const int roundNumber = 2;

    }
}
