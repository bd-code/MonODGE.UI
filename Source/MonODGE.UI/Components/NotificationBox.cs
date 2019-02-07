using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public class NotificationBox : PopUpComponent {
        private string notification;
        private int timeout;
        private Vector2 textPos;
        private Vector2 textDimensions;

        public NotificationBox(StyleSheet style, string text, Vector2 position, int lifetime = 255) : base(style) {
            notification = text;
            timeout = lifetime;

            textDimensions = Style.Font?.MeasureString(notification) ?? Vector2.Zero;

            Dimensions = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)textDimensions.X + MathHelper.Max(Style.BorderTileWidth * 2, Style.Padding * 2),
                MathHelper.Max(Style.BorderTileHeight * 2, (int)textDimensions.Y + Style.Padding * 2)
                );
        }

        public NotificationBox(CityUIManager manager, string text, Vector2 position, int lifetime = 255) : 
            this(manager.GlobalStyle, text, position, lifetime) {
            manager.Add(this);
        }


        public override void Update() {
            timeout--;

            // Update Text Position
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPos = new Vector2(
                    Dimensions.X + Style.Padding,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }

            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPos = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }
            else { // Right
                textPos = new Vector2(
                    Dimensions.Width - textDimensions.X - Style.Padding + Dimensions.X,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }

            // Reduce background opacity.
            Color bgcolor = Style.BackgroundColor;
            bgcolor.A = (byte)timeout;
            Style.BackgroundColor = bgcolor;

            // Darken text and corner color.
            Color textcolor = Style.TextColor;
            if (textcolor.R > 0) {
                textcolor.R -= 1;
                textcolor.G -= 1;
                textcolor.B -= 1;
                textcolor.A -= 1;
            }
            Style.TextColor = textcolor;
            Style.BorderColor = textcolor;

            if (timeout == 0)
                Close();
        }

        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            batch.DrawString(Style.Font, notification, textPos, Style.TextColor);
        }
    }
}
