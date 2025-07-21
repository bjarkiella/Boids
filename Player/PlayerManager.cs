using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class PlayerManager: IEntityManager
    {
        private PlayerEntity _player;
        private BoidManager _boidManager;
        private Texture2D _playerTexture;
        public PlayerManager(Texture2D texture, BoidManager boidManager)
        {
            _playerTexture= texture;
            _boidManager = boidManager;
        }
        public void SpawnPlayer()
        {
            Vector2 spawnPoint = new Vector2(Constants.PWidth/2,Constants.PHeight/2);
            Vector2 spawnVel = new Vector2(0, 0); 
            PlayerEntity newPlayer = new PlayerEntity(_playerTexture, spawnPoint, spawnVel);
        }
        public void Update(GameTime gameTime)
        {
           // Lets chase some boiiididds 
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}