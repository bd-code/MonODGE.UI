using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CityUI.Components {
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
                (int)textDimensions.X + (Style.Corners?.Width ?? Style.Padding * 2),
                Style.Corners?.Height ?? ((int)textDimensions.Y + Style.Padding * 2)
                );
            
            // This should be using TextAlign property.
            textPos = new Vector2(
                (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
        }

        public override void Update() {
            timeout--;

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
            Style.CornerColor = textcolor;

            if (timeout == 0)
                Close();
        }

        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawCorners(batch);

            Vector2 textPos;

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

            batch.DrawString(Style.Font, notification, textPos, Style.TextColor);
        }
    }
}
