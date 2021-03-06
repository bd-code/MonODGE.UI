﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string text;

        private Vector2 textPosition;
        private Vector2 shadowPosition;

        private MoveType movetype;
        private float bounceVelocity;

        public PopText(StyleSheet style, string message, Vector2 position, int lifetime = 80, MoveType motion = MoveType.Static)
            : base(style){
            text = message;

            if (motion == MoveType.Bouncing)
                Timeout = 90;
            else
                Timeout = lifetime;

            Dimensions = new Rectangle((int)position.X, (int)position.Y, 0, 0);

            textPosition = position;
            shadowPosition = new Vector2(position.X + 1, position.Y + 1);

            bounceVelocity = 0.0f;
            movetype = motion;
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
            if (Timeout > 0) {
                textPosition.Y -= 0.5f;
                shadowPosition.Y -= 0.5f;
                Rectangle d = Dimensions;
                d.Y = (int)textPosition.Y;
                Dimensions = d;
                Timeout -= 1;
            }
        }


        private void UpdateFalling() {
            if (Timeout > 0) {
                textPosition.Y += 0.5f;
                shadowPosition.Y += 0.5f;
                Rectangle d = Dimensions;
                d.Y = (int)textPosition.Y;
                Dimensions = d;
                Timeout -= 1;
            }
        }


        private void UpdateBouncing() {
            /* 
            We will need to set timeout to a static value, say 90 frames, in
            constructor if motion == Bouncing. 
            Then hard code velocities at certain intervals, like:
            drop 2px/frame for first 10 frames, then jump 4px/frame next 5 frames,
            then drop .. for next .., then jump back up, then fall to rest.
            Should jump first, like FF6.
            Redirects at:
            40, 20, 20, 5, end at last 5.
            */
            if (Timeout > 60) {
                // First 30 frames. Initial fall.
                textPosition.Y += bounceVelocity;
                shadowPosition.Y += bounceVelocity;
                Timeout -= 1;
                bounceVelocity += 0.05f;
                if (Timeout == 60)
                    bounceVelocity = -1.5f;
            }

            else if (Timeout > 0) {
                // Next 50 frames. Covers both "bounces".
                bounceVelocity += 0.1f;
                textPosition.Y += bounceVelocity;
                shadowPosition.Y += bounceVelocity;
                Timeout -= 1;

                // End of the first bounce, setup velocity for 2nd bounce.
                if (Timeout == 30)
                    bounceVelocity = -1.0f;
            }
        }


        public override void Draw(SpriteBatch batch) {
            if (Timeout > 0) {
                batch.DrawString(Style.Font, text, shadowPosition, Style.FooterColor);
                batch.DrawString(Style.Font, text, textPosition, Style.TextColor);
            }
        }
    }
}
