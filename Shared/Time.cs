using Microsoft.Xna.Framework;
namespace Boids {

    internal static class Time {
        public static float Delta {get; private set;}
        public static float Total {get; private set;}
        public static float Scale {get; private set;}
        
        public static void Update(GameTime gt){
            Delta = (float)gt.ElapsedGameTime.TotalSeconds * Scale;
            Total = (float)gt.TotalGameTime.TotalSeconds;
        }
    }
}
