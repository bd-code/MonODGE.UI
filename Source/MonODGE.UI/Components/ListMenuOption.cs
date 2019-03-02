using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public abstract class AbstractListMenuOption : Component {
        protected ListMenu parent;
        public event EventHandler Submit;

        public AbstractListMenuOption(EventHandler action) {
            Dimensions = new Rectangle(0, 0, 1, 1);
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

    /////////////////////////////////////////////////////////////////

    public class TextListMenuOption : AbstractListMenuOption {
        public string Text { get; protected set; }
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public TextListMenuOption(string optionText, EventHandler action) : base(action) {
            Text = optionText;
            Dimensions = new Rectangle();
        }

        public override void OnMove() {
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


        public override void Refresh() {
            textDimensions = Style.Font?.MeasureString(Text) ?? new Vector2(1, 8);

            // Set Width + Height based on textDimensions.
            // X + Y placement won't matter, as the ListMenuOptionPanel will set these.
            Dimensions = new Rectangle(
                0, 0,
                (int)textDimensions.X + MathHelper.Max(Style.BorderTileWidth * 2, Style.Padding * 2),
                (int)textDimensions.Y + Style.Padding * 4
            );
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
    }
}
