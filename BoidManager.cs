using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    internal class BoidManager
    {
        List<BoidEntity> _boids = new List<BoidEntity>();
        private Texture2D _boidTexture;

        public BoidManager(Texture2D texture)
        {
            _boidTexture = texture;
        }
        public void SpawnBoid()
        {
            BoidEntity newBoid = new BoidEntity(_boidTexture, Utils.RandomSpawnPosition(), Utils.InitialSpeed(), Utils.InitialAngle());
            _boids.Add(newBoid);
        }
        public void Update(GameTime gt)
        {
            Vector2 direction;
            foreach (BoidEntity b in _boids)
            {
                direction = new Vector2(MathF.Cos(b.angle), MathF.Sin(b.angle));
                b.position += direction * b.speed;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (BoidEntity b in _boids)
            {
                sb.Draw(b.texture, b.position, Color.CornflowerBlue);
            }
        }
    }
}