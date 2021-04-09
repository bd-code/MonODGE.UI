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

        protected StyledText _text;
        protected Point textPoint;

        protected string[] footerStrings;
        protected Vector2[] footerDimensions;

        public bool ShowMultiPageFooter { get; set; }

        public string Text {
            get {
                if (messages != null && messageIndex < messages.Length)
                    return messages[messageIndex];
                else
                    return string.Empty;
            }
        }

        public DialogBox(StyleSheet style, Rectangle area, string text) :
            this(style, area, new string[] { text }) { }

        public DialogBox(StyleSheet style, Rectangle area, string[] texts) 
            : base(style) {
            messages = texts;
            messageIndex = 0;

            footerStrings = new string[3]{
                "<< . . .",
                $"[Page i of {messages.Length}]",
                ". . . >>"
                };

            footerDimensions = new Vector2[] {
                Style.FooterFont?.MeasureString(footerStrings[0]) ?? Vector2.Zero,
                Style.FooterFont?.MeasureString(footerStrings[1]) ?? Vector2.Zero,
                Style.FooterFont?.MeasureString(footerStrings[2]) ?? Vector2.Zero
            };
            
            _text = new StyledText(Style, messages[messageIndex]);
            Dimensions = area;  // This calls calcTextPoint();
            ShowMultiPageFooter = true;
        }


        public override void OnStyleChanged() {
            _text.OnStyleChanged();
            calcTextPoint();
            base.OnStyleChanged();
        }


        public override void OnResize() {
            calcTextPoint();
            base.OnResize();
        }


        public override void OnSubmit() {
            messageIndex++;
            if (messageIndex >= messages.Length) 
                Close();
            else
                OnTextChanged();
            base.OnSubmit();
        }

        /// <summary>
        /// This is called when the dialog page is changed and new text displays.
        /// It is not called on style or position changes to the current text, only when a
        /// completely new string displays.
        /// </summary>
        public void OnTextChanged() {
            _text = new StyledText(Style, messages[messageIndex]);
            calcTextPoint();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler TextChanged;


        public override void Update() {
            if (CheckSubmit) {
                OnSubmit();
            }

            else if (messageIndex > 0 && OdgeUIInput.LEFT) {
                messageIndex--;
                OnTextChanged();
            }

            else if (messageIndex < messages.Length - 1 && OdgeUIInput.RIGHT) {
                messageIndex++;
                OnTextChanged();
            }

            else if (CheckCancel) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawBG(batch);
            DrawBorders(batch);

            if (messageIndex < messages.Length) {
                _text.Draw(batch, new Rectangle(Dimensions.Location + textPoint, Dimensions.Size));
                if (messages.Length > 1)
                    drawFooter(batch, Dimensions);
            }
        }


        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Rectangle where = new Rectangle(parentRect.Location + Dimensions.Location, Dimensions.Size);
            DrawBG(batch, where);
            DrawBorders(batch, where);

            if (messageIndex < messages.Length) {
                _text.Draw(batch, new Rectangle(where.Location + textPoint, where.Size));
                if (messages.Length > 1)
                    drawFooter(batch, where);
            }
        }


        private void drawFooter(SpriteBatch batch, Rectangle where) {
            // << . . .
            if (messageIndex > 0)
                batch.DrawString(Style.FooterFont, "<< . . .",
                    new Vector2(
                        where.X + Style.PaddingLeft,
                        where.Bottom - footerDimensions[0].Y - Style.PaddingBottom),
                    Style.FooterColor);

            // [Page i of n]
            if (ShowMultiPageFooter) {
                footerStrings[1] = $"[Page {messageIndex + 1} of {messages.Length}]";
                batch.DrawString(Style.FooterFont, footerStrings[1],
                    new Vector2(
                        where.Center.X - (footerDimensions[1].X / 2),
                        where.Bottom - footerDimensions[1].Y - Style.PaddingBottom),
                    Style.FooterColor);
            }

            // . . . >>
            if (messageIndex < messages.Length - 1)
                batch.DrawString(Style.FooterFont, ". . . >>",
                    new Vector2(
                        where.Right - footerDimensions[2].X - Style.PaddingRight,
                        where.Bottom - footerDimensions[2].Y - Style.PaddingBottom
                        ),
                    Style.FooterColor);
        }


        private void calcTextPoint() {
            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT)
                textPoint.X = Style.PaddingLeft;
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER)
                textPoint.X = Width / 2 - (_text.Width / 2);
            else // Right
                textPoint.X = Width - _text.Width - Style.PaddingRight;

            // Vertical
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                textPoint.Y = Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                textPoint.Y = Height / 2 - (_text.Height / 2);
            else // Bottom
                textPoint.Y = Height - _text.Height - (int)footerDimensions[0].Y - Style.PaddingBottom;
        }
    }
}
