using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GenreShifterProt4
{
    public class Genre
    {
        public string name { get; set; }
        public Texture2D background { get; set; }
        public Texture2D enemy { get; set; }
        public Texture2D platform { get; set; }
        public string abillity { get; set; }
        public bool canUp { get; set; }

        public Genre(string name, Texture2D background, Texture2D enemy, Texture2D platform, string abillity, bool canUp)
        {
            this.name = name;
            this.background = background;
            this.enemy = enemy;
            this.platform = platform;
            this.abillity = abillity;
            this.canUp = canUp;
        }
    }
}