using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Boids.Background
{
    internal class BackgroundResources
    {
        public Texture2D Sky { get; init; }
        public Texture2D TreeSheet { get; init; }
        public List<Rectangle> TreeFrames { get; init; }
        public Texture2D TreeBackground { get; init; }
        public Texture2D TreeDarkBackground { get; init; }
        public Texture2D LargeCloudSheet { get; init; }
        public Texture2D SmallCloudSheet { get; init; }
        public List<Rectangle> LargeCloudFrames { get; init; }
        public List<Rectangle> SmallCloudFrames { get; init; }
    }
}
