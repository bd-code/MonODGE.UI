﻿using System;
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
        private string notification;
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public NoteBox(StyleSheet style, string text, Vector2 position, int lifetime = 355) : base(style) {
            notification = text;
            Timeout = lifetime;

            textDimensions = Style.Font?.MeasureString(notification) ?? Vector2.Zero;

            Dimensions = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)textDimensions.X + Style.PaddingLeft + Style.PaddingRight,
                (int)textDimensions.Y + Style.PaddingTop + Style.PaddingBottom
                );
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
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            batch.DrawString(Style.Font, notification, textPosition, Style.TextColor);
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