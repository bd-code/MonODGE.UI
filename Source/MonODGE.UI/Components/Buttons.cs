using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public class OdgeButtonFactory {
        public static TextButton Create(OdgeComponent parent, string text, EventHandler func) {
            return new TextButton(parent.Style.Clone(), text, func);
        }

        public static ImageButton Create(OdgeComponent parent, Texture2D image, EventHandler func) {
            return new ImageButton(parent.Style.Clone(), image, func);
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public class TextButton : OdgeButton {
        private StyledText text;
        private Point textPoint;

        protected override int MinWidth {
            get {
                if (Style != null)
                    return text.Width + Style.PaddingLeft + Style.PaddingRight;
                else return 0;
            }
        }
        protected override int MinHeight {
            get {
                if (Style != null)
                    return text.Height + Style.PaddingTop + Style.PaddingBottom;
                else return 0;
            }
        }

        public TextButton(StyleSheet style, string optionText, EventHandler action) : base(style, action) {
            text = new StyledText(style, optionText);
            text.StyleMode = StyledText.StyleModes.Unselected;
            Dimensions = new Rectangle(0, 0, MinWidth, MinHeight);
        }


        public override void OnStyleChanged() {
            text.Style = Style;
            calcTextPoint();
            base.OnStyleChanged();
        }


        public override void OnSelected() {
            text.StyleMode = StyledText.StyleModes.Selected;
            base.OnSelected();
        }
        public override void OnUnselected() {
            text.StyleMode = StyledText.StyleModes.Unselected;
            base.OnUnselected();
        }


        public override void Draw(SpriteBatch batch) {
            DrawBG(batch);

            if (IsSelected) 
                DrawBorders(batch);            
            else 
                DrawCorners(batch);
            
            text.Draw(batch, new Rectangle(Dimensions.Location + textPoint, Dimensions.Size));
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Rectangle where = new Rectangle(parentRect.Location + Dimensions.Location, Dimensions.Size);
            DrawBG(batch, where);

            if (IsSelected) 
                DrawBorders(batch, where);            
            else 
                DrawCorners(batch, where);
            
            text.Draw(batch, new Rectangle(where.Location + textPoint, where.Size));
        }


        private void calcTextPoint() {
            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT)
                textPoint.X = Style.PaddingLeft;
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER)
                textPoint.X = Width / 2 - (text.Width / 2);
            else // Right
                textPoint.X = Width - text.Width - Style.PaddingRight;

            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                textPoint.Y = Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                textPoint.Y = Height / 2 - (text.Height / 2);
            else // Bottom
                textPoint.Y = Height - text.Height - Style.PaddingBottom;
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public class ImageButton : OdgeButton {
        private Texture2D texture;
        private Rectangle dstRect;
        private Rectangle srcRect;

        protected override int MinWidth {
            get {
                if (Style != null)
                    return srcRect.Width + Style.PaddingLeft + Style.PaddingRight;
                else return srcRect.Width;
            }
        }
        protected override int MinHeight {
            get {
                if (Style != null)
                    return srcRect.Height + Style.PaddingTop + Style.PaddingBottom;
                else return srcRect.Height;
            }
        }

        public ImageButton(StyleSheet style, Texture2D image, EventHandler action)
            : base(style, action) {
            texture = image;
            srcRect = new Rectangle(0, 0, image.Width, image.Height);
            dstRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            Dimensions = new Rectangle(0, 0, MinWidth, MinHeight);
        }

        public ImageButton(StyleSheet style, Texture2D image, Rectangle sourceRect, EventHandler action)
            : base(style, action) {
            texture = image;
            srcRect = sourceRect;
            dstRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            Dimensions = new Rectangle(0, 0, MinWidth, MinHeight);
        }


        public override void OnStyleChanged() {
            // Reset Dimensions by simply setting same values. Property will take care of resizing.
            Dimensions = new Rectangle(X, Y, Width, Height);
            calcImgPoint();
            base.OnStyleChanged();
        }

        
        public override void OnResize() {
            calcImgPoint();
            base.OnResize();
        }


        public override void Draw(SpriteBatch batch) {
            Rectangle dst = new Rectangle(Dimensions.Location + dstRect.Location, dstRect.Size);
            DrawBG(batch);

            if (IsSelected) {
                DrawBorders(batch);
                batch.Draw(texture, dst, srcRect, Color.White);
            }
            else {
                DrawCorners(batch);
                batch.Draw(texture, dst, srcRect, Color.Gray);
            }
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            // RELATIVE IMG POS IS BROKEN HERE!!
            Rectangle where = new Rectangle(parentRect.Location + Dimensions.Location, Dimensions.Size);
            DrawBG(batch, where);

            if (IsSelected) {
                DrawBorders(batch, where);
                batch.Draw(texture, 
                    new Rectangle(where.Location + dstRect.Location, dstRect.Size), 
                    srcRect, Color.White);
            }
            else {
                DrawCorners(batch, where);
                batch.Draw(texture, 
                    new Rectangle(where.Location + dstRect.Location, dstRect.Size), 
                    srcRect, Color.Gray);
            }
        }


        private void calcImgPoint() {
            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT) 
                dstRect.X = Style.PaddingLeft;            
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER) 
                dstRect.X = Width / 2 - (srcRect.Width / 2);            
            else // Right
                dstRect.X = Width - srcRect.Width - Style.PaddingRight;

            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                dstRect.Y = Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                dstRect.Y = Height / 2 - (srcRect.Height / 2);
            else // Bottom
                dstRect.Y = Height - srcRect.Height - Style.PaddingBottom;
        }
    }

}
