using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    /// <summary>
    /// A multi-page text display box, intended for NPC dialog.
    /// </summary>
    public class DialogBox : OdgeControl {
        protected string[] dialog;
        protected int dialogIndex;

        protected Vector2 textPosition;
        protected Vector2 textDimensions;

        public string Text {
            get {
                if (dialog != null && dialogIndex < dialog.Length)
                    return dialog[dialogIndex];
                else
                    return string.Empty;
            }
        }

        public DialogBox(StyleSheet style, Rectangle area, string line, bool canCancel = false) :
            this(style, area, new string[] { line }, canCancel) { }

        public DialogBox(StyleSheet style, Rectangle area, string[] text, bool canCancel = false) 
            : base(style) {
            dialog = text;
            dialogIndex = 0;
            textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
            repositionText();
            Dimensions = area;
            IsCancelable = canCancel;
        }


        public override void OnStyleSet() {
            if (dialog != null && !string.IsNullOrEmpty(dialog[dialogIndex])) {
                textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
                repositionText();
            }
            base.OnStyleSet();
        }


        public override void OnMove() {
            repositionText();
            base.OnMove();
        }


        public override void OnSubmit() {
            dialogIndex++;
            if (dialogIndex >= dialog.Length) 
                Close();
            base.OnSubmit();
        }


        public override void OnCancel() {
            if (IsCancelable)
                Close();
            base.OnCancel();
        }

        /// <summary>
        /// This is called when the dialog page is changed and new text displays.
        /// It is not called on style or position changes to the current text, only when a
        /// completely new string displays.
        /// </summary>
        public void OnTextChanged() {
            textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
            repositionText();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler TextChanged;


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                OnSubmit();
            }

            else if (dialogIndex > 0
                && (_manager.Input.isKeyPress(Keys.Left) || _manager.Input.isKeyDown(Keys.A))) {
                dialogIndex--;
                OnTextChanged();
            }

            else if (dialogIndex < dialog.Length - 1
                && (_manager.Input.isKeyPress(Keys.Right) || _manager.Input.isKeyDown(Keys.D))) {
                dialogIndex++;
                OnTextChanged();
            }

            else if (_manager.Input.isKeyPress(Style.CancelKey)) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);

            if (dialogIndex < dialog.Length) {
                batch.DrawString(Style.Font, dialog[dialogIndex], textPosition, Style.TextColor);

                int pageIndex = dialogIndex + 1;
                string pageString = "[Page " + pageIndex + " of " + dialog.Length + "]";
                batch.DrawString(Style.FooterFont, pageString,
                    new Vector2(Dimensions.Center.X - (Style.FooterFont.MeasureString(pageString).X / 2), Dimensions.Y + Dimensions.Height - 32),
                    Style.FooterColor);

                if (dialogIndex < dialog.Length - 1)
                    batch.DrawString(Style.FooterFont, ". . . >>",
                        new Vector2(Dimensions.X + Dimensions.Width - Style.PaddingAll - 32, Dimensions.Y + Dimensions.Height - 32),
                        Style.FooterColor);

                if (dialogIndex > 0)
                    batch.DrawString(Style.FooterFont, "<< . . .",
                        new Vector2(Dimensions.X + 16, Dimensions.Y + Dimensions.Height - 32),
                        Style.FooterColor);
            }
        }


        private void repositionText() {
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    X + Style.PaddingLeft,
                    Y + Style.PaddingTop
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPosition = new Vector2(
                    (Width - textDimensions.X) / 2 + X,
                    Y + Style.PaddingTop
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Width - textDimensions.X - Style.PaddingRight + X,
                    Y + Style.PaddingTop
                );
            }
        }
    }
}
