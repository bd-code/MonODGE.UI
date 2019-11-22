using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    /// <summary>
    /// A lightweight text display intended for, but not limited to, short-lived 
    /// one-line notifications.
    /// </summary>
    public class NoteBox : OdgePopUp {
        private string note;
        private Vector2 textPosition;
        private Vector2 textDimensions;

        protected override int MinWidth {
            get { return (int)textDimensions.X + Style.PaddingLeft + Style.PaddingRight; }
        }
        protected override int MinHeight {
            get { return (int)textDimensions.Y + Style.PaddingTop + Style.PaddingBottom; }
        }

        public NoteBox(StyleSheet style, string text, Rectangle area, int lifetime = 355) : base(style) {
            note = text;
            Timeout = lifetime;
            textDimensions = Style.Font?.MeasureString(note) ?? Vector2.Zero;
            Dimensions = area;
        }


        public override void OnMove() {
            repositionText();
            base.OnMove();
        }
        public override void OnResize() {
            repositionText();
            base.OnResize();
        }


        public override void Update() {
            Timeout--;

            if (Timeout < 64) {
                Style.BackgroundColor *= 1.0f - (1.0f / (Timeout+1.0f));
                Style.TextColor *= 1.0f - (1.0f / (Timeout+1.0f));
                Style.BorderColor *= 1.0f - (1.0f / (Timeout+1.0f));
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            batch.DrawString(Style.Font, note, textPosition, Style.TextColor);
        }


        private void repositionText() {
            float nx, ny = 0;

            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT)
                nx = X + Style.PaddingLeft;
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER)
                nx = Dimensions.Center.X - (textDimensions.X / 2);
            else  // Right
                nx = Dimensions.Right - textDimensions.X - Style.PaddingRight;


            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                ny = Y + Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                ny = Dimensions.Center.Y - (textDimensions.Y / 2);
            else // Bottom
                ny = Dimensions.Bottom - textDimensions.Y - Style.PaddingBottom;

            textPosition = new Vector2(nx, ny);
        }
    }
}
