﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    public class QuestionBox : OdgeControl {
        public enum AnswerType { Affirmative, Negative, Unanswered }
        public AnswerType Answer { get; private set; }

        private AbstractMenuOption optionYes;
        private AbstractMenuOption optionNo;
        private bool isYesSelected;
        
        private AlignedText _text;
        private Vector2 textPosition;

        public QuestionBox(StyleSheet style, Rectangle area, string message, AbstractMenuOption yesOption, AbstractMenuOption noOption)
            : base(style) {
            Answer = AnswerType.Unanswered;
            _text = new AlignedText(Style.Font, message, Style.TextAlignH, 0);
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


        public override void OnOpened() {
            repositionOptions();
            base.OnOpened();
        }


        public override void OnStyleChanged() {
            if (_text != null) {
                _text.AlignText(Style.TextAlignH);
                repositionText();
            }
            base.OnStyleChanged();
        }


        public override void OnMove() {
            repositionOptions();
            repositionText();
            base.OnMove();
        }
        public override void OnResize() {
            repositionOptions();
            repositionText();
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
            if (CheckSubmit) {
                OnSubmit();
            }

            else if (!isYesSelected && OdgeInput.LEFT) {
                optionNo.OnUnselected();
                isYesSelected = true;
                optionYes.OnSelected();
            }

            else if (isYesSelected && OdgeInput.RIGHT) {
                optionYes.OnUnselected();
                isYesSelected = false;
                optionNo.OnSelected();
            }

            else if (CheckCancel) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            _text.Draw(batch, textPosition, Style.TextColor);
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
                nx = Dimensions.Center.X - (_text.Width / 2);
            else  // Right
                nx = Dimensions.Right - _text.Width - Style.PaddingRight;


            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                ny = Y + Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                ny = Dimensions.Center.Y - (_text.Height / 2);
            else // Bottom
                ny = Dimensions.Bottom - _text.Height - Style.PaddingBottom;

            textPosition = new Vector2(nx, ny);
        }
    }
}
