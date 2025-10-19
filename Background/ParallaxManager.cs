using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Shared;

namespace Boids.Background
{
    internal class ParallaxManager
    {
        private readonly List<ParallaxLayer> _layers = [];

        public ParallaxManager(Texture2D cloudTexture, List<Rectangle> cloudFrames)
        {
            // Create 3 layers: far (slow), mid, near (fast)
            _layers.Add(new ParallaxLayer(cloudTexture, cloudFrames, 0.3f, 5.0f));  // Far
            _layers.Add(new ParallaxLayer(cloudTexture, cloudFrames, 0.6f, 3.0f));  // Mid
            _layers.Add(new ParallaxLayer(cloudTexture, cloudFrames, 1.0f, 2.0f));  // Near
        }

        public void Update()
        {
            foreach (var layer in _layers)
                layer.Update();
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw far to near (so near clouds are on top)
            foreach (var layer in _layers)
                layer.Draw(sb);
        }
    }}

