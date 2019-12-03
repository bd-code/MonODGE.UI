using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public abstract class AbstractMenuOption : OdgeComponent {
        public AbstractMenuOption(EventHandler action) {
            Submit += action;
        }

        /// <summary>
        /// This is called when the user presses the key assigned in Style.SubmitKey.
        /// </summary>
        public virtual void OnSubmit() { Submit?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Submit;

        /// <summary>
        /// This is called when the user presses the key assigned in Style.CancelKey.
        /// </summary>
        public virtual void OnCancel() { Cancel?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Cancel;

        /// <summary>
        /// This is called when an option is highlighted in the ListMenu.
        /// </summary>
        public virtual void OnSelected() { Selected?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Selected;

        /// <summary>
        /// This is called when an option is unhighlighted (another option is selected) in the
        /// ListMenu.
        /// </summary>
        public virtual void OnUnselected() { Unselected?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Unselected;

        internal virtual void Update(bool selected) { }
        internal virtual void Draw(SpriteBatch batch, bool selected) { }
    }

    ///////////////////////////////////////////////////////////////////////////

    public class TextMenuOption : AbstractMenuOption {
        public string Text { get; protected set; }
        private Vector2 textPosition;
        private Vector2 textDimensions;

        protected override int MinWidth {
            get {
                if (Style != null)
                    return (int)textDimensions.X + Style.PaddingLeft + Style.PaddingRight;
                else return 0;
            }
        }
        protected override int MinHeight {
            get {
                if (Style != null)
                    return (int)textDimensions.Y + (Style.PaddingTop + Style.PaddingBottom) * 2;
                else return 0;
            }
        }

        public TextMenuOption(string optionText, EventHandler action) : base(action) {
            Text = optionText;
        }


        public override void OnStyleChanged() {
            if (!string.IsNullOrEmpty(Text)) {
                textDimensions = Style.Font?.MeasureString(Text) ?? new Vector2(1, 8);

                // Reset Dimensions by simply setting same values. Property will take care of resizing.
                Dimensions = new Rectangle(X, Y, Width, Height);

                repositionText();
            }
            base.OnStyleChanged();
        }


        public override void OnMove() {
            repositionText();
            base.OnMove();
        }
        public override void OnResize() {
            repositionText();
            base.OnResize();
        }


        internal override void Draw(SpriteBatch batch, bool selected) {
            DrawCanvas(batch);

            if (selected) {
                DrawBorders(batch);
                batch.DrawString(Style.Font, Text, textPosition, Style.SelectedTextColor);
            }
            else {
                DrawCorners(batch);
                batch.DrawString(Style.Font, Text, textPosition, Style.UnselectedTextColor);
            }
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

    ///////////////////////////////////////////////////////////////////////////

    public class ImageMenuOption : AbstractMenuOption {
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

        public ImageMenuOption(Texture2D image, EventHandler action)
            : base(action) {
            texture = image;
            srcRect = new Rectangle(0, 0, image.Width, image.Height);
            dstRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
        }

        public ImageMenuOption(Texture2D image, Rectangle sourceRect, EventHandler action)
            : base(action) {
            texture = image;
            srcRect = sourceRect;
            dstRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
        }


        public override void OnStyleChanged() {
            // Reset Dimensions by simply setting same values. Property will take care of resizing.
            Dimensions = new Rectangle(X, Y, Width, Height);
            base.OnStyleChanged();
        }


        public override void OnMove() {
            repositionImage();
            base.OnMove();
        }
        public override void OnResize() {
            repositionImage();
            base.OnResize();
        }


        internal override void Draw(SpriteBatch batch, bool selected) {
            DrawCanvas(batch);

            if (selected) {
                DrawBorders(batch);
                batch.Draw(texture, dstRect, srcRect, Color.White);
            }
            else {
                DrawCorners(batch);
                batch.Draw(texture, dstRect, srcRect, Color.Gray);
            }
        }


        private void repositionImage() {
            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT) 
                dstRect.X = X + Style.PaddingLeft;            
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER) 
                dstRect.X = Dimensions.Center.X - (srcRect.Width / 2);            
            else // Right
                dstRect.X = Dimensions.Right - srcRect.Width - Style.PaddingRight;

            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                dstRect.Y = Y + Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                dstRect.Y = Dimensions.Center.Y - (srcRect.Height / 2);
            else // Bottom
                dstRect.Y = Dimensions.Bottom - srcRect.Height - Style.PaddingBottom;
        }
    }

}
