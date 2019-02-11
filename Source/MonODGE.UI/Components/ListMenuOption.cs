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
        public event EventHandler Confirm;

        public AbstractListMenuOption(EventHandler action) {
            Confirm += action;
        }

        public void OnConfirm() {
            Confirm?.Invoke(this, new EventArgs());
        }
        
        internal virtual void Draw(SpriteBatch batch, bool selected) { }
        public virtual void Measure() { }
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


        public override void Measure() {
            textDimensions = Style.Font?.MeasureString(Text) ?? new Vector2(1, 8);
            
            Dimensions = new Rectangle(
                0, // <-- X-Pos will be set by ListMenu.
                0, // <-- Y-Pos will be changed frequently by ListMenu.
                (int)textDimensions.X + MathHelper.Max(Style.BorderTileWidth * 2, Style.Padding * 2),
                MathHelper.Max(Style.BorderTileHeight * 2, (int)textDimensions.Y + Style.Padding * 2)
                );

            Refresh();
        }


        public override void Refresh() {            
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


        internal override void Draw(SpriteBatch batch, bool selected) {
            DrawCanvas(batch);
            Refresh(); // REMOVE ONCE FIXED!

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
