using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    /// <summary>
    /// An unframed (windowless) text display intended for damage numbers and other 
    /// "point" text, exclamations, etc.
    /// </summary>
    public class PopText : OdgePopUp {
        public enum MoveType {
            Static, Rising, Falling, Bouncing
        };

        private StyledText _text;
        private MoveType movetype;
        private int bounce_c;
        private float bounce_y;
        private float bounce_vy;
        private float bounce_ay;


        public PopText(StyleSheet style, string message, Vector2 position, int lifetime = 80, MoveType motion = MoveType.Static)
            : base(style){
            _text = new StyledText(style, message);
            Dimensions = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            Timeout = lifetime;

            movetype = motion;
            if (movetype == MoveType.Bouncing) {
                bounce_c = 1;
                bounce_y = 0.0f;
                bounce_vy = -3.9f;
                bounce_ay = 0.26f;
            }
        }


        public override void Update() {
            if (movetype == MoveType.Rising)
                UpdateRising();
            if (movetype == MoveType.Falling)
                UpdateFalling();
            if (movetype == MoveType.Bouncing)
                UpdateBouncing();
            else { // Static, stays put.
                if (Timeout > 0)
                    Timeout -= 1;
            }
        }


        private void UpdateRising() {
            if (Timeout >= 0 && Timeout % 3 == 0)
                Y -= 1;
            Timeout -= 1;
        }
        private void UpdateFalling() {
            if (Timeout >= 0 && Timeout % 3 == 0)
                Y += 1;
            Timeout -= 1;
        }
        private void UpdateBouncing() {
            bounce_y += bounce_vy;
            bounce_vy += bounce_ay;
            _text.Y = (int)Math.Round(bounce_y);

            if (_text.Y >= 0) {
                if (bounce_c == 1) {
                    bounce_c++;
                    bounce_y = 0.0f;
                    bounce_vy = -3.6f;
                    bounce_ay = 0.36f;
                }
                else if (bounce_c == 2) {
                    bounce_c++;
                    bounce_y = 0.0f;
                    bounce_vy = -3.5f;
                    bounce_ay = 0.70f;
                }
                else {
                    movetype = MoveType.Static;
                }
            }

            if (Timeout > 1) // Do NOT hit zero while bouncing!
                Timeout--;
        }

        public override void Draw(SpriteBatch batch) {
            if (Timeout > 0) 
                _text.Draw(batch, Dimensions);
        }
        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            if (Timeout > 0)
                _text.Draw(batch, new Rectangle(Dimensions.Location + parentRect.Location, Dimensions.Size));
        }
    }
}
