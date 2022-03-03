using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenreShifterProt4.Sprites
{
    public class Enemy : Sprite
    {
        public Enemy(Texture2D texture) : base(texture)
        {

        }

        public override void Update(GameTime gameTime, Sprite[] enemies)
        {
            Position += Velocity;
            //Velocity = Vector2.Zero;
        }
    }
}