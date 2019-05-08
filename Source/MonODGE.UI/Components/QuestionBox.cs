using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Components {
    public class QuestionBox : OdgeControl {
        public enum AnswerType { Affirmative, Negative, Unanswered }
        public AnswerType Answer { get; private set; }

        private AbstractMenuOption optionYes;
        private AbstractMenuOption optionNo;
        private bool isYesSelected;

        private string dialog;
        private Vector2 textDimensions;
        private Vector2 textPosition;

        public QuestionBox(StyleSheet style, Rectangle area, string message, AbstractMenuOption yesOption, AbstractMenuOption noOption)
            : base(style) {
            Answer = AnswerType.Unanswered;
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
            base.OnStyleSet();
        }


        public override void OnMove() {
            repositionOptions();
            base.OnMove();
        }
        public override void OnResize() {
            repositionOptions();
            base.OnResize();
        }

        public override void OnSubmit() {
            if (isYesSelected) {
                Answer = AnswerType.Affirmative;
                optionYes.OnSubmit();
            }
            else {
                Answer = AnswerType.Negative;
                optionNo.OnSubmit();
            }
            base.OnSubmit();
        }


        public override void OnCancel() {
            if (isYesSelected) {
                optionYes.OnUnselected();
                isYesSelected = false;
                optionNo.OnSelected();
            }
            base.OnCancel();
        }


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                OnSubmit();
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
                bottomspace = _manager.ScreenHeight - (Y + Height);
            
            if (Y >= bottomspace) {
                // Draw options on top
                optionYes.Dimensions = new Rectangle(
                    Dimensions.Center.X - optionYes.Width - Style.SpacingH / 2,
                    Y - optionYes.Height - Style.SpacingV,
                    optionYes.Width, 
                    optionYes.Height
                    );

                optionNo.Dimensions = new Rectangle(
                    Dimensions.Center.X + Style.SpacingH / 2,
                    Y - optionYes.Height - Style.SpacingV,
                    optionYes.Width,
                    optionYes.Height
                    );
            }

            else {
                // Draw options on bottom
                optionYes.Dimensions = new Rectangle(
                    Dimensions.Center.X - optionYes.Width - Style.SpacingH / 2,
                    Y + Height + Style.SpacingV,
                    optionYes.Width,
                    optionYes.Height
                    );

                optionNo.Dimensions = new Rectangle(
                    Dimensions.Center.X + Style.SpacingH / 2,
                    Y + Height + Style.SpacingV,
                    optionYes.Width,
                    optionYes.Height
                    );
            }

            repositionText();
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
