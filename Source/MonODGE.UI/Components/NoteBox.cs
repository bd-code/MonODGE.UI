
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    /// <summary>
    /// A lightweight text display intended for, but not limited to, short-lived 
    /// one-line notifications.
    /// </summary>
    public class NoteBox : OdgePopUp {
        private StyledText _text;
        private Point textPoint;

        protected override int MinWidth {
            get { return _text.Width + Style.PaddingLeft + Style.PaddingRight; }
        }
        protected override int MinHeight {
            get { return _text.Height + Style.PaddingTop + Style.PaddingBottom; }
        }

        public NoteBox(StyleSheet style, string text, Rectangle area, int lifetime = 360) : base(style) {
            _text = new StyledText(style, text);
            Timeout = lifetime;
            Dimensions = area;
        }


        public override void OnStyleChanged() {
            calcTextPoint();
            base.OnStyleChanged();
        }
        public override void OnMove() {
            calcTextPoint();
            base.OnMove();
        }
        public override void OnResize() {
            calcTextPoint();
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
            DrawBG(batch);
            DrawBorders(batch);
            _text.Draw(batch, new Rectangle(Dimensions.Location + textPoint, Dimensions.Size));
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Rectangle where = new Rectangle(parentRect.Location + Dimensions.Location, Dimensions.Size);
            DrawBG(batch, where);
            DrawBorders(batch, where);
            _text.Draw(batch, new Rectangle(where.Location + textPoint, where.Size));
        }


        private void calcTextPoint() {
            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT)
                textPoint.X = Style.PaddingLeft;
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER)
                textPoint.X = Width / 2 - (_text.Width / 2);
            else // Right
                textPoint.X = Width - _text.Width - Style.PaddingRight;

            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                textPoint.Y = Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                textPoint.Y = Height / 2 - (_text.Height / 2);
            else // Bottom
                textPoint.Y = Height - _text.Height - Style.PaddingBottom;
        }
    }
}
