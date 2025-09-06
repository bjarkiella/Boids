using Microsoft.Xna.Framework;
namespace Boids.Shared {

    internal static class Time {
        public static float Delta {get; private set;}
        public static float Total {get; private set;}
        
        public static void Update(GameTime gt)
        {
            Delta = (float)gt.ElapsedGameTime.TotalSeconds;
            Total = (float)gt.TotalGameTime.TotalSeconds;
        }
    }
}
