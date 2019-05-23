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

        protected string[] footerStrings;
        protected Vector2[] footerDimensions;

        public string Text {
            get {
                if (dialog != null && dialogIndex < dialog.Length)
                    return dialog[dialogIndex];
                else
                    return string.Empty;
            }
        }

        public DialogBox(StyleSheet style, Rectangle area, string line) :
            this(style, area, new string[] { line }) { }

        public DialogBox(StyleSheet style, Rectangle area, string[] text) 
            : base(style) {
            dialog = text;
            dialogIndex = 0;

            footerStrings = new string[3]{
                "<< . . .",
                "[Page i of n]",
                ". . . >>"
                };

            footerDimensions = new Vector2[] {
                Style.FooterFont?.MeasureString(footerStrings[0]) ?? Vector2.Zero,
                Style.FooterFont?.MeasureString(footerStrings[1]) ?? Vector2.Zero,
                Style.FooterFont?.MeasureString(footerStrings[2]) ?? Vector2.Zero
            };

            textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
            Dimensions = area;  // This calls repositionText();
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

                // << . . .
                if (dialogIndex > 0)
                    batch.DrawString(Style.FooterFont, "<< . . .",
                        new Vector2(
                            Dimensions.X + Style.PaddingLeft, 
                            Dimensions.Bottom - footerDimensions[0].Y - Style.PaddingBottom),
                        Style.FooterColor);

                // [Page i of n]
                if (dialog.Length > 1) {
                    int pageIndex = dialogIndex + 1;
                    footerStrings[1] = "[Page " + pageIndex + " of " + dialog.Length + "]";
                    batch.DrawString(Style.FooterFont, footerStrings[1],
                        new Vector2(
                            Dimensions.Center.X - (footerDimensions[1].X / 2),
                            Dimensions.Bottom - footerDimensions[1].Y - Style.PaddingBottom),
                        Style.FooterColor);
                }

                // . . . >>
                if (dialogIndex < dialog.Length - 1)
                    batch.DrawString(Style.FooterFont, ". . . >>",
                        new Vector2(
                            Dimensions.Right - footerDimensions[2].X - Style.PaddingRight,
                            Dimensions.Bottom - footerDimensions[2].Y - Style.PaddingBottom
                            ),
                        Style.FooterColor);
            }
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
                ny = Dimensions.Bottom - textDimensions.Y - footerDimensions[0].Y - Style.PaddingBottom;

            textPosition = new Vector2(nx, ny);
        }
    }
}
