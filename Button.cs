using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GenreShifterProt4
{
    public class Button
    {
        private readonly Vector2 baseScreenSize;
        private Matrix globalTransformation;
        private readonly Texture2D unPressedTexture;
        private readonly Texture2D pressedTexture;
        //private readonly Vector2 buttonPosition;
        private readonly Rectangle buttonPosition;

        private float secondsSinceLastInput;

        public Texture2D currentTexture;
        public bool isPressed { get; set; }
        public bool isReleasd { get; set; }
        private int releaseHelper;

        public Button(Vector2 baseScreenSize, Matrix globalTransformation, Texture2D unPressedTexture, Texture2D pressedTexture, Rectangle buttonPosition)
        {
            this.baseScreenSize = baseScreenSize;
            this.globalTransformation = Matrix.Invert(globalTransformation);
            this.unPressedTexture = unPressedTexture;
            this.pressedTexture = pressedTexture;
            this.buttonPosition = buttonPosition;
            isPressed = false;
            isReleasd = false;
            releaseHelper = 0;
            currentTexture = unPressedTexture;
            secondsSinceLastInput = float.MaxValue;
        }

        public void Update(GameTime gameTime)
        {
            var secondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            secondsSinceLastInput += secondsElapsed;

            if (secondsSinceLastInput > 0.1) //not pressed
            {
                isPressed = false;
                currentTexture = unPressedTexture;
                if (releaseHelper > 0)
                    isReleasd = true;
            }
            else //pressed
            {
                currentTexture = pressedTexture;
                //System.Diagnostics.Debug.WriteLine("aughhhh");
                isPressed = true;
                releaseHelper++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentTexture, new Vector2(buttonPosition.X, buttonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Generates a GamePadState based on the touch input provided (as applied to the on screen controls) and the gamepad state
        /// </summary>
        public GamePadState GetState(TouchCollection touchState, GamePadState gpState)
        {
            //Work out what buttons are pressed based on the touchState
            Buttons buttonsPressed = 0;

            foreach (var touch in touchState)
            {
                if (touch.State == TouchLocationState.Moved || touch.State == TouchLocationState.Pressed)
                {
                    //Scale the touch position to be in _baseScreenSize coordinates
                    Vector2 pos = touch.Position;
                    Vector2.Transform(ref pos, ref globalTransformation, out pos);

                    if (pos.X >= buttonPosition.X &&
                        pos.X <= buttonPosition.Right &&
                        pos.Y >= buttonPosition.Y &&
                        pos.Y <= buttonPosition.Bottom)
                        secondsSinceLastInput = 0;
                }
            }

            //Combine the buttons of the real gamepad
            var gpButtons = gpState.Buttons;
            buttonsPressed |= (gpButtons.A == ButtonState.Pressed ? Buttons.A : 0);
            buttonsPressed |= (gpButtons.B == ButtonState.Pressed ? Buttons.B : 0);
            buttonsPressed |= (gpButtons.X == ButtonState.Pressed ? Buttons.X : 0);
            buttonsPressed |= (gpButtons.Y == ButtonState.Pressed ? Buttons.Y : 0);

            buttonsPressed |= (gpButtons.Start == ButtonState.Pressed ? Buttons.Start : 0);
            buttonsPressed |= (gpButtons.Back == ButtonState.Pressed ? Buttons.Back : 0);

            buttonsPressed |= gpState.IsButtonDown(Buttons.DPadDown) ? Buttons.DPadDown : 0;
            buttonsPressed |= gpState.IsButtonDown(Buttons.DPadLeft) ? Buttons.DPadLeft : 0;
            buttonsPressed |= gpState.IsButtonDown(Buttons.DPadRight) ? Buttons.DPadRight : 0;
            buttonsPressed |= gpState.IsButtonDown(Buttons.DPadUp) ? Buttons.DPadUp : 0;

            buttonsPressed |= (gpButtons.BigButton == ButtonState.Pressed ? Buttons.BigButton : 0);
            buttonsPressed |= (gpButtons.LeftShoulder == ButtonState.Pressed ? Buttons.LeftShoulder : 0);
            buttonsPressed |= (gpButtons.RightShoulder == ButtonState.Pressed ? Buttons.RightShoulder : 0);

            buttonsPressed |= (gpButtons.LeftStick == ButtonState.Pressed ? Buttons.LeftStick : 0);
            buttonsPressed |= (gpButtons.RightStick == ButtonState.Pressed ? Buttons.RightStick : 0);

            var buttons = new GamePadButtons(buttonsPressed);

            return new GamePadState(gpState.ThumbSticks, gpState.Triggers, buttons, gpState.DPad);
        }
    }
}