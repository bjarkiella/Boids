using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class PredatorManager : IEntityManager
    {
        private List<PredatorEntity> _predator = new List<PredatorEntity>();
        public IReadOnlyList<PredatorEntity> listOfPredators => _predator;
        private BoidManager _boidManager;
        private Texture2D _predatorTexture;
        public PredatorManager(Texture2D texture, BoidManager boidManager)
        {
            _predatorTexture = texture;
            _boidManager = boidManager;
        }
        public void SpawnPred()
        {
            Vector2 spawnPoint = Utils.RandomSpawnPosition();
            Vector2 spawnVel = Utils.InitialVelocity(Utils.InitialAngle(), Utils.InitialSpeed());
            PredatorEntity newPred = new PredatorEntity(_predatorTexture, spawnPoint, spawnVel);
            _predator.Add(newPred);
        }
        public void RemovePred()
        {
            if (_predator.Count > 0)
            {
                _predator.RemoveAt(_predator.Count - 1);
            }
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