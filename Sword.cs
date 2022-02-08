using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GenreShifterProt4.Sprites
{
    public class Sword : Sprite
    {
        public Sword(Texture2D texture)
            : base(texture)
        {

        }


        public override void Update(GameTime gameTime, Sprite[] sprites)
        {
            Position += Velocity;

            Velocity = Vector2.Zero;
        }
    }
}