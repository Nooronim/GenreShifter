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
            //gamePadState = Input.GetState(touchState, GamePad.GetState(PlayerIndex.One));
            //Move();

            //foreach (var sprite in sprites)
            //{
            //    if (sprite == this)
            //        continue;

            //    if (this.Velocity.X > 0 && this.IsTouchingLeft(sprite) ||
            //        this.Velocity.X < 0 && this.IsTouchingRight(sprite))
            //    {
            //        this.Velocity.X = 0;
            //        System.Diagnostics.Debug.WriteLine("hit left or right!");
            //        //check what it hit
            //    }

            //    if (this.Velocity.Y > 0 && this.IsTouchingTop(sprite))
            //    {
            //        this.Velocity.Y = 0;
            //        System.Diagnostics.Debug.WriteLine("touch top");
            //        //check what it hit
            //    }
            //    if (this.Velocity.Y < 0 && this.IsTouchingBottom(sprite))
            //    {
            //        this.Velocity.Y = 0;
            //        //check what it hit
            //    }
            //}

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