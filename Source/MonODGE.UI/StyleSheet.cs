using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI {
    public class StyleSheet {
        public enum TextAlignments {
            LEFT, CENTER, RIGHT
        }

        public Texture2D Background { get; set; }
        public Color BackgroundColor { get; set; }
        
        private Texture2D _borders;
        public Texture2D Borders {
            get { return _borders; }
            set {
                _borders = value;
                BorderSourceRects = new Rectangle[] {
                    new Rectangle(0, 0, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, 0, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, 0, BorderTileWidth, BorderTileHeight),

                    new Rectangle(0, BorderTileHeight, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, BorderTileHeight, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, BorderTileHeight, BorderTileWidth, BorderTileHeight),

                    new Rectangle(0, BorderTileHeight*2, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, BorderTileHeight*2, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, BorderTileHeight*2, BorderTileWidth, BorderTileHeight)
                };
            }
        }
        public Rectangle[] BorderSourceRects { get; private set; }
        public int BorderTileWidth { get { return _borders?.Width / 3 ?? 0; } }
        public int BorderTileHeight { get { return _borders?.Height / 3 ?? 0; } }
        public Color BorderColor { get; set; }

        public SpriteFont HeaderFont { get; set; }
        public Color HeaderColor { get; set; }

        public SpriteFont Font { get; set; }
        public Color TextColor { get; set; }

        public SpriteFont FooterFont { get; set; }
        public Color FooterColor { get; set; }
        
        public Color SelectedTextColor { get; set; }
        public Color UnselectedTextColor { get; set; }

        public TextAlignments TextAlign { get; set; }
        public int Padding { get; set; }
        
        public Keys SubmitKey { get; set; }
        public Keys CancelKey { get; set; }

        
        public StyleSheet(Texture2D background = null, Color? bgcolor = default(Color?),
                          Texture2D borders = null, Color? bordercolor = default(Color?),
                          SpriteFont headerfont = null, Color? headercolor = default(Color?),
                          SpriteFont font = null, Color? textColor = default(Color?), 
                          SpriteFont footerfont = null, Color? footercolor = default(Color?),
                          Color? selectedTextColor = default(Color?), Color? unselectedTextColor = default(Color?),
                          TextAlignments textAlign = TextAlignments.LEFT, int padding = 0
                          ) {

            Background = background;
            BackgroundColor = bgcolor ?? Color.TransparentBlack;

            Borders = borders;
            BorderColor = bordercolor ?? Color.White;

            HeaderFont = headerfont;
            if (HeaderFont == null)
                HeaderFont = font;

            Font = font;

            FooterFont = footerfont;
            if (FooterFont == null)
                FooterFont = font;

            TextColor = textColor ?? Color.White;
            HeaderColor = headercolor ?? Color.White;
            FooterColor = footercolor ?? Color.White;
            SelectedTextColor = selectedTextColor ?? Color.Gold;
            UnselectedTextColor = unselectedTextColor ?? Color.Gray;
            
            TextAlign = textAlign;
            Padding = padding;
        }
        
        public static StyleSheet Default { get { return new StyleSheet(); } }

        public StyleSheet Clone() {
            StyleSheet clone = new StyleSheet(
                Background, BackgroundColor, 
                Borders, BorderColor, 
                HeaderFont, HeaderColor, 
                Font, TextColor, 
                FooterFont, FooterColor, 
                SelectedTextColor, UnselectedTextColor,
                TextAlign, Padding);
            clone.SubmitKey = SubmitKey;
            clone.CancelKey = CancelKey;
            return clone;
        }
    }
}
