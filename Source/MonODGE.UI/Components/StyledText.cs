using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public class StyledText : OdgePopUp {
        private string[] _lines;
        private Vector2[] _positions;

        public enum StyleModes {
            Standard = 0, Header = 1, Footer = 2, Selected = 3, Unselected = 4
        }

        private StyleModes _stymode;
        public StyleModes StyleMode {
            get { return _stymode; }
            set {
                _stymode = value;
                OnStyleChanged();
            }
        }
        private SpriteFont _font {
            get {
                if (StyleMode == StyleModes.Header) return Style.HeaderFont;
                else if (StyleMode == StyleModes.Footer) return Style.FooterFont;
                else return Style.Font;
            }
        }
        private Color _color {
            get {
                if (StyleMode == StyleModes.Header) return Style.HeaderColor;
                else if (StyleMode == StyleModes.Footer) return Style.FooterColor;
                else if (StyleMode == StyleModes.Selected) return Style.SelectedTextColor;
                else if (StyleMode == StyleModes.Unselected) return Style.UnselectedTextColor;
                else return Style.TextColor;
            }
        }


        public StyledText(StyleSheet style, string textblock) : 
            this(style, textblock.Split(new[] { Environment.NewLine }, StringSplitOptions.None)) { }

        public StyledText(StyleSheet style, string[] textlines) : base (style) {
            _lines = textlines;
            _positions = new Vector2[_lines.Length];
            StyleMode = StyleModes.Standard;
            AlignText();
        }


        private void AlignText() {
            float lasty = 0;
            float maxWidth = 0;

            for (int s = 0; s < _lines.Length; s++) {
                Vector2 dims = _font?.MeasureString(_lines[s]) ?? Vector2.Zero;
                _positions[s] = new Vector2(dims.X, lasty);
                if (s > 0)
                    lasty += Style.SpacingV;
                lasty += dims.Y;
                maxWidth = Math.Max(maxWidth, _positions[s].X);
            }

            // At this point _position.X is the width of the string, not it's X position!
            // Alignment X repositioning is below!

            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = 0;
            }
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = (maxWidth - _positions[p].X) / 2;                  
            }
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.RIGHT) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = (maxWidth - _positions[p].X);
            }
            
            // Lastly, calculate new width + height based on alignment and style.
            if (Dimensions == null)
                Dimensions = new Rectangle(0, 0, (int)maxWidth, (int)lasty);
            else
                Dimensions = new Rectangle(X, Y, (int)maxWidth, (int)lasty);
        }


        public override void OnStyleChanged() {
            AlignText();
            base.OnStyleChanged();
        }


        public override void Draw(SpriteBatch batch) {
            for (int d = 0; d < _lines.Length; d++) {
                drawShadows(batch, d, new Vector2(X, Y));
                batch.DrawString(_font, _lines[d], 
                    new Vector2(X, Y) + _positions[d], 
                    _color);
            }
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Vector2 where = new Vector2(X + parentRect.X, Y + parentRect.Y);
            for (int d = 0; d < _lines.Length; d++) {
                drawShadows(batch, d, where);
                batch.DrawString(_font, _lines[d], 
                    where + _positions[d], 
                    _color);
            }
        }


        private void drawShadows(SpriteBatch batch, int index, Vector2 where) {
            for (int s = 0; s < Style.TextShadows.Length; s++) {
                if (Style.TextShadows[s] != Vector2.Zero) {
                    batch.DrawString(_font, _lines[index],
                        where + _positions[index] + Style.TextShadows[s],
                        Style.TextShadowColor);
                }
            }
        }
    }
}
