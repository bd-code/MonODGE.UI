using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Utilities {
    public class AlignedText {
        //public enum Alignments { LEFT, CENTER, RIGHT }
        public int Width { get; private set; }
        public int Height {
            get {
                if (_lines.Length == 0)
                    return 0;
                else {
                    float height = _spacing * (_lines.Length - 1);
                    foreach (Vector2 p in _positions)
                        height += p.Y;
                    return (int)height;
                }
            }
        }

        private string[] _lines;
        private Vector2[] _positions;
        private SpriteFont _font;
        private int _spacing;

        public AlignedText(SpriteFont font, string textblock, StyleSheet.AlignmentsH alignment, int lineSpacing = 0) {
            _font = font;
            _lines = textblock.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            _spacing = lineSpacing;
            _positions = new Vector2[_lines.Length];
            AlignText(alignment);
        }

        public AlignedText(SpriteFont font, string[] textlines, StyleSheet.AlignmentsH alignment, int lineSpacing = 0) {
            _font = font;
            _lines = textlines;
            _spacing = lineSpacing;
            _positions = new Vector2[textlines.Length];
            AlignText(alignment);
        }

        public void AlignText(StyleSheet.AlignmentsH alignment) {
            float lasty = 0;
            float maxWidth = 0;

            for (int s = 0; s < _lines.Length; s++) {
                Vector2 dims = _font.MeasureString(_lines[s]);
                _positions[s] = new Vector2(dims.X, 0);
                _positions[s].Y += lasty;
                lasty += dims.Y + _spacing;
                maxWidth = Math.Max(maxWidth, _positions[s].X);
            }

            // At this point _position.X is the width of the string, not it's X position!
            // Alignment X repositioning is below!

            if (alignment == StyleSheet.AlignmentsH.LEFT) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = 0;
            }
            else if (alignment == StyleSheet.AlignmentsH.CENTER) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = (maxWidth - _positions[p].X) / 2;                  
            }
            else if (alignment == StyleSheet.AlignmentsH.RIGHT) {
                for (int p = 0; p < _positions.Length; p++)
                    _positions[p].X = (maxWidth - _positions[p].X);
            }

            Width = (int)maxWidth;
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color) {
            for (int d = 0; d < _lines.Length; d++) {
                batch.DrawString(_font, _lines[d], _positions[d] + position, color);
            }
        }
    }
}
