using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    public interface IEntityManager
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}