using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO: Change this class so it makes sense the player uses it
namespace Boids
{
    internal class PlayerEntity:BaseEntity
    {
        public PlayerEntity(Texture2D texture, Vector2 position, Vector2 velocity,float visionFactor):base(texture,position,velocity,visionFactor)
        {

        }
        //public override void Update(GameTime gameTime)
        //{
            
        //}
    }
}