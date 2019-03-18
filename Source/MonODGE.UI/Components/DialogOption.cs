using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Components {
    public class DialogOption : OdgeControl {
        private AbstractMenuOption optionYes;
        private AbstractMenuOption optionNo;
        private bool isYesSelected;

        private string dialog;
        private Vector2 textDimensions;
        private Vector2 textPosition;

        public DialogOption(StyleSheet style, Rectangle area, string message, AbstractMenuOption yesOption, AbstractMenuOption noOption)
            : base(style) {
            dialog = message;
            textDimensions = Style.Font?.MeasureString(dialog) ?? Vector2.Zero;
            repositionText();

            // Options need style first.
            optionYes = yesOption;
            if (optionYes.Style == null)
                optionYes.Style = style;

            optionNo = noOption;
            if (optionNo.Style == null)
                optionNo.Style = Style;

            // Init Dimensions on these options.
            optionYes.Dimensions = new Rectangle(0, 0, 1, 1);
            optionNo.Dimensions = new Rectangle(0, 0, 1, 1);
            Dimensions = area;

            // bool isYesSelected is false; optionNo should be selected.
            optionNo.OnSelected();
        }


        public override void Initialize() {
            repositionOptions();
        }


        public override void OnStyleSet() {
            if (!string.IsNullOrEmpty(dialog)) {
                textDimensions = Style.Font?.MeasureString(dialog) ?? Vector2.Zero;
                repositionText();
            }
        }


        public override void OnMove() {
            repositionOptions();
            //repositionText();
        }
        public override void OnResize() {
            repositionOptions();
            //repositionText();
        }


        public override void OnCancel() {
            if (IsCancelable)
                Close();
        }


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                if (isYesSelected)
                    optionYes.OnSubmit();
                else
                    optionNo.OnSubmit();
            }

            else if (!isYesSelected && _manager.Input.isKeyPress(Keys.Left)) {
                optionNo.OnUnselected();
                isYesSelected = true;
                optionYes.OnSelected();
            }

            else if (isYesSelected && _manager.Input.isKeyPress(Keys.Right)) {
                optionYes.OnUnselected();
                isYesSelected = false;
                optionNo.OnSelected();
            }

            else if (_manager.Input.isKeyPress(Style.CancelKey)) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            if (!string.IsNullOrEmpty(dialog))
                batch.DrawString(Style.Font, dialog, textPosition, Style.TextColor);

            optionYes.Draw(batch, isYesSelected);
            optionNo.Draw(batch, !isYesSelected);
        }


        private void repositionOptions() {
            int bottomspace = 0;
            if (_manager != null)
                bottomspace = _manager.ScreenHeight - (Dimensions.Y + Dimensions.Height);
            
            if (Dimensions.Y >= bottomspace) { 
                // Draw on top
                optionYes.Dimensions = new Rectangle(
                    Dimensions.X + Style.PaddingLeft * 4,
                    Dimensions.Y - optionYes.Dimensions.Height - Style.PaddingTop,
                    optionYes.Dimensions.Width, 
                    optionYes.Dimensions.Height
                    );

                optionNo.Dimensions = new Rectangle(
                    Dimensions.X + Dimensions.Width - optionNo.Dimensions.Width - Style.PaddingRight * 4,
                    Dimensions.Y - optionYes.Dimensions.Height - Style.PaddingTop,
                    optionYes.Dimensions.Width,
                    optionYes.Dimensions.Height
                    );
            }

            else {
                // Draw on bottom
                optionYes.Dimensions = new Rectangle(
                    Dimensions.X + Style.PaddingLeft * 4,
                    Dimensions.Y + Dimensions.Height + Style.PaddingBottom,
                    optionYes.Dimensions.Width,
                    optionYes.Dimensions.Height
                    );

                optionNo.Dimensions = new Rectangle(
                    Dimensions.X + Dimensions.Width - optionNo.Dimensions.Width - Style.PaddingRight * 4,
                    Dimensions.Y + Dimensions.Height + Style.PaddingBottom,
                    optionYes.Dimensions.Width,
                    optionYes.Dimensions.Height
                    );
            }

            repositionText();
        }

        private void repositionText() {
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    Dimensions.X + Style.PaddingLeft,
                    Dimensions.Y + Style.PaddingTop
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPosition = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    Dimensions.Y + Style.PaddingTop
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Dimensions.Width - textDimensions.X - Style.PaddingRight + Dimensions.X,
                    Dimensions.Y + Style.PaddingTop
                );
            }
        }
    }
}
