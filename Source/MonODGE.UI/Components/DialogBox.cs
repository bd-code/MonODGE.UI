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
        protected bool isCancelable;

        protected Vector2 textPosition;
        protected Vector2 textDimensions;

        public DialogBox(StyleSheet style, string line, Rectangle area, bool canCancel = false) :
            this(style, new string[] { line }, area, canCancel) { }

        public DialogBox(StyleSheet style, string[] text, Rectangle area, bool canCancel = false) 
            : base(style) {
            dialog = text;
            dialogIndex = 0;
            OnTextChanged();
            isCancelable = canCancel;
            Dimensions = area;
        }


        public override void OnStyleSet() {
            if (dialog != null && !string.IsNullOrEmpty(dialog[dialogIndex]))
                OnTextChanged();
        }


        public override void OnMove() {
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    Dimensions.X + Style.Padding,
                    Dimensions.Y + Style.Padding
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPosition = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    Dimensions.Y + Style.Padding
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Dimensions.Width - textDimensions.X - Style.Padding + Dimensions.X,
                    Dimensions.Y + Style.Padding
                );
            }
        }


        public override void OnSubmit() {
            dialogIndex++;
            if (dialogIndex >= dialog.Length) 
                Close();            
        }


        public override void OnCancel() {
            if (isCancelable)
                Close();            
        }


        public void OnTextChanged() {
            textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
            OnMove();
        }


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
                        new Vector2(Dimensions.X + Dimensions.Width - Style.Padding - 32, Dimensions.Y + Dimensions.Height - 32),
                        Style.FooterColor);

                if (dialogIndex > 0)
                    batch.DrawString(Style.FooterFont, "<< . . .",
                        new Vector2(Dimensions.X + 16, Dimensions.Y + Dimensions.Height - 32),
                        Style.FooterColor);
            }
        }
    }
}
