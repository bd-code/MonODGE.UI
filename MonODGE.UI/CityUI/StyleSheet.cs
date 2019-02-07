using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CityUI {
    public class StyleSheet {
        public enum TextAlignments {
            LEFT, CENTER, RIGHT
        }

        public Texture2D Background { get; set; }
        public Color BackgroundColor { get; set; }

        public Texture2D Corners { get; set; }
        public Rectangle[] CornerSourceRectangles { get; private set; }
        public int CornerTileWidth { get { return Corners?.Width / 2 ?? 0; } }
        public int CornerTileHeight { get { return Corners?.Height / 2 ?? 0; } }
        public Color CornerColor { get; set; }

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
                          Texture2D corners = null, Color? cornercolor = default(Color?),
                          SpriteFont headerfont = null, Color? headercolor = default(Color?),
                          SpriteFont font = null, Color? textColor = default(Color?), 
                          SpriteFont footerfont = null, Color? footercolor = default(Color?),
                          Color? selectedTextColor = default(Color?), Color? unselectedTextColor = default(Color?),
                          TextAlignments textAlign = TextAlignments.LEFT, int padding = 0
                          ) {

            Background = background;
            BackgroundColor = bgcolor ?? Color.TransparentBlack;

            Corners = corners;
            if (Corners != null) {
                int width = Corners.Width / 2;
                int height = Corners.Height / 2;

                CornerSourceRectangles = new Rectangle[] {
                    new Rectangle(0, 0, width, height),
                    new Rectangle(width, 0, width, height),
                    new Rectangle(0, height, width, height),
                    new Rectangle(width, height, width, height)
                };
            }
            CornerColor = cornercolor ?? Color.Gray;

            HeaderFont = headerfont;
            if (HeaderFont == null)
                HeaderFont = font;

            Font = font;

            FooterFont = footerfont;
            if (FooterFont == null)
                FooterFont = font;

            TextColor = textColor ?? Color.White;
            HeaderColor = TextColor;
            FooterColor = TextColor;
            SelectedTextColor = selectedTextColor ?? Color.Gold;
            UnselectedTextColor = unselectedTextColor ?? Color.Gray;
            
            TextAlign = textAlign;
            Padding = padding;
        }


        public StyleSheet Clone() {
            StyleSheet clone = new StyleSheet(
                Background, BackgroundColor, 
                Corners, CornerColor, 
                HeaderFont, HeaderColor, 
                Font, TextColor, 
                FooterFont, FooterColor, 
                SelectedTextColor, UnselectedTextColor,
                TextAlign, Padding);
            clone.SubmitKey = SubmitKey;
            clone.CancelKey = CancelKey;
            return clone;
        }

        /*public static StyleSheet Default {
            get {
                StyleSheet sheet = new StyleSheet();
                //sheet.Background = new Texture2D();
                return sheet;
            }
        }*/
    }
}
