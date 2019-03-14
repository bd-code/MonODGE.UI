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
    public class NotificationBox : OdgePopUp {
        private string notification;
        private int timeout;
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public NotificationBox(StyleSheet style, string text, Vector2 position, int lifetime = 355) : base(style) {
            notification = text;
            timeout = lifetime;

            textDimensions = Style.Font?.MeasureString(notification) ?? Vector2.Zero;

            Dimensions = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)textDimensions.X + Style.Padding * 2,
                (int)textDimensions.Y + Style.Padding
                );
        }


        public override void OnMove() {            
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    Dimensions.X + Style.Padding,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPosition = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Dimensions.Width - textDimensions.X - Style.Padding + Dimensions.X,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }
        }


        public override void OnResize() {
            OnMove();
        }


        public override void Update() {
            timeout--;

            // Reduce background opacity.
            Color bgcolor = Style.BackgroundColor;
            bgcolor.A = (byte)MathHelper.Max(0, bgcolor.A - 1);
            Style.BackgroundColor = bgcolor;

            // Darken text color.
            Color textcolor = Style.TextColor;
            textcolor.R = (byte)MathHelper.Max(0, textcolor.R - 1);
            textcolor.G = (byte)MathHelper.Max(0, textcolor.G - 1);
            textcolor.B = (byte)MathHelper.Max(0, textcolor.B - 1);
            textcolor.A = (byte)MathHelper.Max(0, textcolor.A - 1);
            Style.TextColor = textcolor;

            // Darken border color.
            Color bordercolor = Style.BorderColor;
            bordercolor.R = (byte)MathHelper.Max(0, bordercolor.R - 1);
            bordercolor.G = (byte)MathHelper.Max(0, bordercolor.G - 1);
            bordercolor.B = (byte)MathHelper.Max(0, bordercolor.B - 1);
            bordercolor.A = (byte)MathHelper.Max(0, bordercolor.A - 1);
            Style.BorderColor = bordercolor;

            if (timeout == 0)
                Close();
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            batch.DrawString(Style.Font, notification, textPosition, Style.TextColor);
        }
    }
}
