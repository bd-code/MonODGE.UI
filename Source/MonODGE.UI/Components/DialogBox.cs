using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    /// <summary>
    /// A multi-page text display box, intended for NPC dialog.
    /// </summary>
    public class DialogBox : OdgeControl {
        protected string[] messages;
        protected int messageIndex;

        protected AlignedText display;
        protected Vector2 textPosition;

        protected string[] footerStrings;
        protected Vector2[] footerDimensions;

        public string Text {
            get {
                if (messages != null && messageIndex < messages.Length)
                    return messages[messageIndex];
                else
                    return string.Empty;
            }
        }

        public DialogBox(StyleSheet style, Rectangle area, string line) :
            this(style, area, new string[] { line }) { }

        public DialogBox(StyleSheet style, Rectangle area, string[] text) 
            : base(style) {
            messages = text;
            messageIndex = 0;

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
            
            display = new AlignedText(Style.Font, messages[messageIndex], Style.TextAlignH, 0);
            Dimensions = area;  // This calls repositionText();
        }


        public override void OnStyleSet() {
            if (messages != null && !string.IsNullOrEmpty(messages[messageIndex])) {
                repositionText();
            }
            base.OnStyleSet();
        }


        public override void OnMove() {
            repositionText();
            base.OnMove();
        }


        public override void OnSubmit() {
            messageIndex++;
            if (messageIndex >= messages.Length) 
                Close();
            base.OnSubmit();
        }

        /// <summary>
        /// This is called when the dialog page is changed and new text displays.
        /// It is not called on style or position changes to the current text, only when a
        /// completely new string displays.
        /// </summary>
        public void OnTextChanged() {
            display = new AlignedText(Style.Font, messages[messageIndex], Style.TextAlignH, 0);
            repositionText();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler TextChanged;


        public override void Update() {
            if (CheckSubmit) {
                OnSubmit();
            }

            else if (messageIndex > 0 && OdgeInput.LEFT) {
                messageIndex--;
                OnTextChanged();
            }

            else if (messageIndex < messages.Length - 1 && OdgeInput.RIGHT) {
                messageIndex++;
                OnTextChanged();
            }

            else if (CheckCancel) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);

            if (messageIndex < messages.Length) {
                display.Draw(batch, textPosition, Style.TextColor);

                // << . . .
                if (messageIndex > 0)
                    batch.DrawString(Style.FooterFont, "<< . . .",
                        new Vector2(
                            Dimensions.X + Style.PaddingLeft, 
                            Dimensions.Bottom - footerDimensions[0].Y - Style.PaddingBottom),
                        Style.FooterColor);

                // [Page i of n]
                if (messages.Length > 1) {
                    int pageIndex = messageIndex + 1;
                    footerStrings[1] = "[Page " + pageIndex + " of " + messages.Length + "]";
                    batch.DrawString(Style.FooterFont, footerStrings[1],
                        new Vector2(
                            Dimensions.Center.X - (footerDimensions[1].X / 2),
                            Dimensions.Bottom - footerDimensions[1].Y - Style.PaddingBottom),
                        Style.FooterColor);
                }

                // . . . >>
                if (messageIndex < messages.Length - 1)
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
                nx = Dimensions.Center.X - (display.Width / 2);
            else  // Right
                nx = Dimensions.Right - display.Width - Style.PaddingRight;


            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                ny = Y + Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                ny = Dimensions.Center.Y - (display.Height / 2);
            else // Bottom
                ny = Dimensions.Bottom - display.Height - footerDimensions[0].Y - Style.PaddingBottom;

            textPosition = new Vector2(nx, ny);
        }
    }
}
