using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GenreShifterProt4.Sprites
{
    public class Player : Sprite
    {
        //private float movement;
        //private string whichBtn;
        //private TouchCollection touchState;

        //private GamePadState gamePadState;
        //private VirtualGamePad virtualGamePad;

        public Player(Texture2D texture) 
          : base(texture)
        {

        }

        public override void Update(GameTime gameTime, Sprite[] sprites)
        {

            switch (Direction)
            {
                case "right":
                    TextureSpriteEffects = SpriteEffects.None;
                    break;
                case "left":
                    TextureSpriteEffects = SpriteEffects.FlipHorizontally;
                    break;
                default:
                    //TextureSpriteEffects = SpriteEffects.None;
                    break;
            }

            Position += Velocity;

            Velocity = Vector2.Zero;
        }
    }
}