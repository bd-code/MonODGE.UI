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

        public virtual void InitMenuOption() { }
        internal virtual void Draw(SpriteBatch batch, bool selected) { }
    }

    /////////////////////////////////////////////////////////////////

    public class TextListMenuOption : AbstractListMenuOption {
        public string Text { get; protected set; }
        private Vector2 textDimensions;

        public TextListMenuOption(string optionText, EventHandler action) : base(action) {
            Text = optionText;
            Dimensions = new Rectangle();
        }


        public override void InitMenuOption() {
            if (Style.Font == null)
                throw new NullReferenceException("TextListMenuOption.Style.Font is null.");

            textDimensions = Style.Font.MeasureString(Text);
            
            Dimensions = new Rectangle(
                0,
                0, // <-- This will be changed every Draw().
                (int)textDimensions.X + Style.Padding * 2,
                (int)textDimensions.Y * 2
                );
        }


        internal override void Draw(SpriteBatch batch, bool selected) {
            DrawCanvas(batch);

            Vector2 TextPos;

            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                TextPos = new Vector2(
                    Dimensions.X + Style.Padding,
                    Dimensions.Y + (textDimensions.Y / 2)
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                TextPos = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    Dimensions.Y + (textDimensions.Y / 2)
                );
            }
            else { // Right
                TextPos = new Vector2(
                    Dimensions.X + Dimensions.Width - textDimensions.X - Style.Padding,
                    Dimensions.Y + (textDimensions.Y / 2)
                );
            }

            if (selected) {
                DrawBorders(batch);
                batch.DrawString(Style.Font, Text, TextPos, Style.SelectedTextColor);
            }
            else {
                DrawCorners(batch);
                batch.DrawString(Style.Font, Text, TextPos, Style.UnselectedTextColor);
            }
        }
    }
}
