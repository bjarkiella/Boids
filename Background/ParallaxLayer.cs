using System;
using System.Collections.Generic;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{

    internal class ParallaxLayer(Texture2D texture, List<Rectangle> frames, float speedMultiplier, float spawnRate)
    {
        private readonly Texture2D _texture = texture;
        private readonly List<Rectangle> _frames = frames;
        private readonly float _speedMultiplier = speedMultiplier;  // 0.2f for far, 0.5f for mid, 1.0f for near
        private readonly float _spawnRate = spawnRate;        // How often to spawn entities

        private readonly List<ParallaxEntity> _parralaxEntity = [];
        private float _timeSinceLastSpawn;

        protected static float Dt => Time.Delta;
        public void Update()
        {
            _timeSinceLastSpawn += Dt;
            if (_timeSinceLastSpawn >= _spawnRate)
            {
                SpawnParralaxEntity();
                _timeSinceLastSpawn = 0f;
            }
            // Removing offscreen entitites and updating
            List<ParallaxEntity> delList = [];
            foreach(ParallaxEntity entity in _parralaxEntity)
            {
                entity.Update(_speedMultiplier);
                if (entity.IsOffScreen())
                    delList.Add(entity);
            }

            foreach(ParallaxEntity entity in delList) _parralaxEntity.Remove(entity);

        }

        public void Draw(SpriteBatch sb)
        {
            foreach(ParallaxEntity entity in _parralaxEntity)
            {
                entity.Draw(sb);
            }
        }

        private void SpawnParralaxEntity()
        {
            Rectangle randomFrame = _frames[Utils.RandomIntRange(0,_frames.Count-1)];
            float randomY = Utils.RandomFloatRange(0,Constants.ActiveHeight/1.5f);
            float speed = Utils.RandomFloatRange(-50f,50f);

            if (MathF.Abs(speed) < 10f)
                speed = speed > 0? 10f :-10f;

            float scale = Utils.RandomFloatRange(1.5f, 3.0f);

            float spawnX;
            if (speed > 0f)
                spawnX = -randomFrame.Width * scale/2f; // Spawn off left edge
            else
                spawnX = Constants.ActiveWidth + randomFrame.Width*scale/2f; // Spawn of right edge

            Vector2 spawnPos = new(spawnX,randomY);

            ParallaxEntity entity = new(_texture,randomFrame,spawnPos,speed,scale);
            _parralaxEntity.Add(entity);
        }
    }
}
