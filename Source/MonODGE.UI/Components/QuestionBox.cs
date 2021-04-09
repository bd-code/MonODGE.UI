
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    public class QuestionBox : OdgeControl {
        public enum AnswerType { Affirmative, Negative, Unanswered }
        public AnswerType Answer { get; private set; }

        private OdgeButton btnYes;
        private OdgeButton btnNo;
        
        private DialogBox dialog;
        protected Point textPoint;

        private bool _btnInText;
        public bool ButtonsInText {
            get { return _btnInText; }
            set {
                _btnInText = value;
                calcBtnPoints();
            }
        }

        private bool _btnOnTop;
        public bool ButtonsOnTop {
            get { return _btnOnTop; }
            set {
                _btnOnTop = value;
                calcBtnPoints();
            }
        }

        public QuestionBox(StyleSheet style, Rectangle area, string message, OdgeButton yesBtn, OdgeButton noBtn)
            : base(style) {
            Answer = AnswerType.Unanswered;
            _btnInText = true;
            _btnOnTop = false;
            dialog = new DialogBox(Style, new Rectangle(Point.Zero, area.Size), message);
            dialog.ShowMultiPageFooter = false;

            // Options need style first.
            btnYes = yesBtn;
            if (btnYes.Style == null)
                btnYes.Style = style;

            btnNo = noBtn;
            if (btnNo.Style == null)
                btnNo.Style = style;

            // Init Dimensions on these options.
            btnYes.Dimensions = new Rectangle(0, 0, 1, 1);
            btnNo.Dimensions = new Rectangle(0, 0, 1, 1);
            Dimensions = area;
            calcBtnPoints();

            // At first optionNo should be selected.
            btnNo.OnSelected();
        }


        public override void OnStyleChanged() {
            dialog.OnStyleChanged();
            btnYes.OnStyleChanged();
            btnNo.OnStyleChanged();
            calcBtnPoints();
            base.OnStyleChanged();
        }


        public override void OnResize() {
            dialog.Dimensions = new Rectangle(Point.Zero, Dimensions.Size);
            calcBtnPoints();
            base.OnResize();
        }


        public override void OnSubmit() {
            if (btnYes.IsSelected) {
                Answer = AnswerType.Affirmative;
                btnYes.OnSubmit();
            }
            else {
                Answer = AnswerType.Negative;
                btnNo.OnSubmit();
            }
            base.OnSubmit();
        }


        public override void OnCancel() {
            if (btnYes.IsSelected) {
                btnYes.OnUnselected();
                btnNo.OnSelected();
            }
            base.OnCancel();
        }


        public override void Update() {
            if (CheckSubmit) {
                OnSubmit();
            }

            else if (!btnYes.IsSelected && OdgeUIInput.LEFT) {
                btnNo.OnUnselected();
                btnYes.OnSelected();
            }

            else if (btnYes.IsSelected && OdgeUIInput.RIGHT) {
                btnYes.OnUnselected();
                btnNo.OnSelected();
            }

            else if (CheckCancel) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            dialog.Draw(batch, Dimensions);
            btnYes.Draw(batch, Dimensions);
            btnNo.Draw(batch, Dimensions);
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Rectangle where = new Rectangle(parentRect.Location + Dimensions.Location, Dimensions.Location);
            dialog.Draw(batch, where);
            btnYes.Draw(batch, where);
            btnNo.Draw(batch, where);
        }


        private void calcBtnPoints() {
            if (ButtonsInText)
                calcBtnInPoints();
            else
                calcBtnOutPoints();
        }


        private void calcBtnInPoints() {
            // Inside DialogBox uses Padding to anchor.
            int by = 0;
            if (ButtonsOnTop) {
                by = Style.PaddingTop;
            }
            else {
                by = Height - btnYes.Height - Style.PaddingBottom;
            }

            btnYes.Dimensions = new Rectangle(
                Style.PaddingLeft,
                by, btnYes.Width, btnYes.Height
                );

            btnNo.Dimensions = new Rectangle(
                Width - btnNo.Width - Style.PaddingRight,
                by, btnYes.Width, btnYes.Height
                );
        }


        private void calcBtnOutPoints() {
            // Outside DialogBox uses Spacing to float.  
            int by = 0;            
            if (ButtonsOnTop) 
                by = 0 - btnYes.Height - Style.SpacingV;
            else
                by = Height + Style.SpacingV;

            btnYes.Dimensions = new Rectangle(
                (Width - Style.SpacingH) / 2 - btnYes.Width,
                by, btnYes.Width, btnYes.Height
                );

            btnNo.Dimensions = new Rectangle(
                (Width + Style.SpacingH) / 2,
                by, btnYes.Width, btnYes.Height
                );
        }
    }
}
