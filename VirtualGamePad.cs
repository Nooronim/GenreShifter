using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GenreShifterProt4
{
    public class VirtualGamePad
    {
        private readonly Vector2 baseScreenSize;
        private Matrix globalTransformation;
        private readonly Texture2D gamePadTexture;
        private readonly Texture2D connectorTexture;
        private readonly Texture2D actionButtonTexture;


        private float secondsSinceLastInput;
        private float opacity;

        public VirtualGamePad(Vector2 baseScreenSize, Matrix globalTransformation, Texture2D gamePadTexture, Texture2D connectorTexture, Texture2D actionButtonTexture)
        {
            this.baseScreenSize = baseScreenSize;
            this.globalTransformation = Matrix.Invert(globalTransformation);
            this.gamePadTexture = gamePadTexture;
            this.connectorTexture = connectorTexture;
            this.actionButtonTexture = actionButtonTexture;
            secondsSinceLastInput = float.MaxValue;
        }

        public void NotifyPlayerIsMoving()
        {
            secondsSinceLastInput = 0;
        }

        public void Update(GameTime gameTime)
        {
            var secondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            secondsSinceLastInput += secondsElapsed;

            //If the player is moving, fade the controls out
            // otherwise, if they haven't moved in 2 seconds, fade the controls back in
            if (secondsSinceLastInput < 2)
                opacity = Math.Max(0.3f, opacity - secondsElapsed * 4);
            else
                opacity = Math.Min(1, opacity + secondsElapsed * 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var spriteCenter = new Vector2(64, 64);
            var color = Color.Multiply(Color.White, opacity);

            spriteBatch.Draw(gamePadTexture, new Vector2(256, baseScreenSize.Y - 384), null, color, 0, spriteCenter, 1, SpriteEffects.None, 0);//up
            spriteBatch.Draw(gamePadTexture, new Vector2(128, baseScreenSize.Y - 256), null, color, -MathHelper.PiOver2, spriteCenter, 1, SpriteEffects.None, 0);//left
            spriteBatch.Draw(gamePadTexture, new Vector2(384, baseScreenSize.Y - 256), null, color, MathHelper.PiOver2, spriteCenter, 1, SpriteEffects.None, 0);//right
            spriteBatch.Draw(gamePadTexture, new Vector2(256, baseScreenSize.Y - 128), null, color, 2*MathHelper.PiOver2, spriteCenter, 1, SpriteEffects.None, 0);//down
            spriteBatch.Draw(connectorTexture, new Vector2(256, baseScreenSize.Y - 256), null, color, 0, spriteCenter, 1, SpriteEffects.None, 0);//connector
            spriteBatch.Draw(actionButtonTexture, new Vector2(baseScreenSize.X - 320, baseScreenSize.Y - 352), null, color, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);//action
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

                    //System.Diagnostics.Debug.WriteLine(pos.X.ToString(), pos.Y.ToString());

                    if (pos.X >= 0 &&
                        pos.X < 320 &&
                        pos.Y > baseScreenSize.Y - 320 &&
                        pos.Y < baseScreenSize.Y - 192)
                        buttonsPressed |= Buttons.DPadLeft; //left dPad

                    else if (pos.X >= 320 &&
                        pos.X < 512 &&
                        pos.Y > baseScreenSize.Y - 320 &&
                        pos.Y < baseScreenSize.Y - 192)
                        buttonsPressed |= Buttons.DPadRight; //right dPad

                    if (pos.X > 128 &&
                        pos.X <= 384 &&
                        pos.Y >= baseScreenSize.Y - 512 &&
                        pos.Y < baseScreenSize.Y - 256)
                        buttonsPressed |= Buttons.DPadUp; //up dPad

                    if (pos.X > 128 &&
                        pos.X <= 384 &&
                        pos.Y > baseScreenSize.Y - 256 &&
                        pos.Y <= baseScreenSize.Y)
                        buttonsPressed |= Buttons.DPadDown; //down dPad

                    if (pos.X >= baseScreenSize.X - 320 &&
                        pos.Y >= baseScreenSize.Y - 352)
                        buttonsPressed |= Buttons.A; //action
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