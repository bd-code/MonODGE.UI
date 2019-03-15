using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public abstract class AbstractMenuOption : OdgeComponent {
        protected ListMenu parent;
        public event EventHandler Submit;

        public AbstractMenuOption(EventHandler action) {
            Submit += action;
        }

        /// <summary>
        /// This is called when the user presses the key assigned in Style.SubmitKey.
        /// </summary>
        public virtual void OnSubmit() {
            Submit?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// This is called when the user presses the key assigned in Style.CancelKey.
        /// </summary>
        public virtual void OnCancel() { }

        /// <summary>
        /// This is called when an option is highlighted in the ListMenu.
        /// </summary>
        public virtual void OnSelected() { }

        /// <summary>
        /// This is called when an option is unhighlighted (another option is selected) in the
        /// ListMenu.
        /// </summary>
        public virtual void OnUnselected() { }
        
        internal virtual void Update(bool selected) { }
        internal virtual void Draw(SpriteBatch batch, bool selected) { }
    }

    ///////////////////////////////////////////////////////////////////////////

    public class TextMenuOption : AbstractMenuOption {
        public string Text { get; protected set; }
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public override Rectangle Dimensions {
            get { return base.Dimensions; }
            set {
                // First check if value is too small for the text, and if so, resize.
                int minWidth = 0, minHeight = 0;
                if (Style != null) {
                    minWidth = (int)textDimensions.X + Style.Padding * 2;
                    minHeight = (int)textDimensions.Y + Style.Padding * 4;
                }

                base.Dimensions = new Rectangle(
                    value.X, value.Y, 
                    MathHelper.Max(value.Width, minWidth), 
                    MathHelper.Max(value.Height, minHeight)
                    );
            }
        }

        public TextMenuOption(string optionText, EventHandler action) : base(action) {
            Text = optionText;
        }


        public override void OnStyleSet() {
            if (!string.IsNullOrEmpty(Text)) {
                textDimensions = Style.Font?.MeasureString(Text) ?? new Vector2(1, 8);

                // Reset Dimensions by simply setting same values. Property will take care of resizing.
                Dimensions = new Rectangle(
                    Dimensions.X, Dimensions.Y,
                    Dimensions.Width, Dimensions.Height
                    );

                repositionText();
            }
        }


        public override void OnMove() { repositionText(); }
        public override void OnResize() { repositionText(); }


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
            // Text Positioning
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
                    Dimensions.X + Dimensions.Width - textDimensions.X - Style.Padding,
                    (Dimensions.Height - textDimensions.Y) / 2 + Dimensions.Y
                );
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public class ImageMenuOption : AbstractMenuOption {
        private Texture2D texture;
        private Rectangle dstRect;
        private Rectangle srcRect;

        public override Rectangle Dimensions {
            get { return base.Dimensions; }

            set {
                // Make sure it's at least as big as the texture.
                int minWidth = 0, minHeight = 0;
                if (Style != null) {
                    minWidth = srcRect.Width + Style.Padding * 2;
                    minHeight = srcRect.Height + Style.Padding * 2;
                }

                base.Dimensions = new Rectangle(
                    value.X, value.Y,
                    MathHelper.Max(value.Width, minWidth),
                    MathHelper.Max(value.Height, minHeight)
                    );
            }
        }

        public ImageMenuOption(Texture2D image, Rectangle sourceRect, EventHandler action)
            : base(action) {
            texture = image;
            srcRect = sourceRect;
            dstRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
        }


        public override void OnStyleSet() {
            // Reset Dimensions by simply setting same values. Property will take care of resizing.
            Dimensions = new Rectangle(
                Dimensions.X, Dimensions.Y,
                Dimensions.Width, Dimensions.Height
                );
        }


        public override void OnMove() {
            repositionImage();
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
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                dstRect = new Rectangle(
                    Dimensions.X + Style.Padding,
                    Dimensions.Y + Style.Padding,
                    srcRect.Width,
                    srcRect.Height
                    );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                dstRect = new Rectangle(
                    (Dimensions.Width - srcRect.Width) / 2 + Dimensions.X,
                    Dimensions.Y + Style.Padding,
                    srcRect.Width,
                    srcRect.Height
                    );
            }
            else { // Right
                dstRect = new Rectangle(
                    Dimensions.X + Dimensions.Width - srcRect.Width - Style.Padding,
                    Dimensions.Y + Style.Padding,
                    srcRect.Width,
                    srcRect.Height
                    );
            }
        }
    }

}
