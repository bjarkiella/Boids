using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{
    internal class ParallaxManager
    {
        private readonly List<ParallaxLayer> _layers = [];

        public ParallaxManager(Texture2D texture, List<Rectangle> cloudFrames)
        {
            // Create 3 layers: far (slow), mid, near (fast)
            _layers.Add(new ParallaxLayer(texture, cloudFrames, 0.3f, 5.0f));  // Far
            _layers.Add(new ParallaxLayer(texture, cloudFrames, 0.6f, 3.0f));  // Mid
            _layers.Add(new ParallaxLayer(texture, cloudFrames, 1.0f, 2.0f));  // Near
        }

        public void Update()
        {
            foreach (var layer in _layers)
                layer.Update();
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var layer in _layers)
                layer.Draw(sb);
        }

        public List<Rectangle> GetEntityBounds()
        {
            List<Rectangle> entitesBounds= [];
            foreach (var layer in _layers)
                entitesBounds.AddRange(layer.GetLayerBounds());
            return entitesBounds;
        }
    }
}

