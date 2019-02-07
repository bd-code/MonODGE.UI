using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public class DialogBox : Control {
        // Should have a textwrap function.
        private string[] dialog;
        private int dialogIndex;
        private bool isCancelable;
        
        private Vector2 textDimensions;

        public DialogBox(StyleSheet style, string line, Rectangle area, bool canCancel = false) :
            this(style, new string[] { line }, area, canCancel) { }

        public DialogBox(StyleSheet style, string[] text, Rectangle area, bool canCancel = false) 
            : base(style) {
            dialog = text;
            dialogIndex = 0;
            isCancelable = canCancel;

            Dimensions = area;
            textDimensions = Style.Font?.MeasureString(dialog[dialogIndex]) ?? Vector2.Zero;
        }

        public DialogBox(CityUIManager manager, string line, Rectangle area, bool canCancel = false) :
            this(manager.GlobalStyle, new string[] { line }, area, canCancel) {
            manager.Add(this);
        }

        public DialogBox(CityUIManager manager, string[] text, Rectangle area, bool canCancel = false) :
            this(manager.GlobalStyle, text, area, canCancel) {
            manager.Add(this);
        }


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                dialogIndex++;
                if (dialogIndex >= dialog.Length) {
                    Close();
                }
            }
            else if (dialogIndex > 0
                && (_manager.Input.isKeyPress(Keys.Left) || _manager.Input.isKeyDown(Keys.A))) {
                dialogIndex--;
                textDimensions = Style.Font.MeasureString(dialog[dialogIndex]);
            }
            else if (dialogIndex < dialog.Length - 1
                && (_manager.Input.isKeyPress(Keys.Right) || _manager.Input.isKeyDown(Keys.D))) {
                dialogIndex++;
                textDimensions = Style.Font.MeasureString(dialog[dialogIndex]);
            }
            else if (isCancelable && _manager.Input.isKeyPress(Style.CancelKey)) {
                Close();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawCorners(batch);
            
            if (dialogIndex < dialog.Length) {

                Vector2 textPos;

                if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                    textPos = new Vector2(Dimensions.X + Style.Padding, Dimensions.Y + Style.Padding);
                }

                else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                    textPos = new Vector2(
                        (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X, 
                        Dimensions.Y + Style.Padding
                    );
                }
                else { // Right
                    textPos = new Vector2(
                        Dimensions.Width - textDimensions.X - Style.Padding + Dimensions.X,
                        Dimensions.Y + Style.Padding
                    );
                }

                batch.DrawString(Style.Font, dialog[dialogIndex], textPos, Style.TextColor);

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
