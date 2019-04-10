using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Components {
    public class TextBox : OdgeControl {
        public enum CharsAllowed {
            Any, Alpha, Numeric, AlphaNumeric
        }

        private KeyboardState _keystate;
        private KeyboardState _oldstate;

        private string _text;
        public string Text {
            get { return _text; }
            set {
                _text = value;
                OnTextChanged();
            }
        }

        public int TextLength { get { return Text?.Length ?? 0; } }
        public int MaxLength { get; private set; }
        public CharsAllowed InputRules { get; private set; }

        private Texture2D[] _frame;
        private Vector2 textPosition;

        public TextBox(StyleSheet style, Rectangle area, 
            CharsAllowed allowed = CharsAllowed.Any, string text = "", int maxLength = 255) : base(style) {

            // Adjust init height
            int height2 = style.PaddingBottom + style.PaddingTop + (int)(style.Font?.MeasureString("T").Y ?? 16);
            area.Height = MathHelper.Max(area.Height, height2);
            Dimensions = area;

            Text = text ?? string.Empty;
            MaxLength = maxLength;
            InputRules = allowed;

            _keystate = Keyboard.GetState();
            _oldstate = _keystate;
            _frame = new Texture2D[2];
        }


        public override void Initialize() {
            initFrame();
        }


        private void initFrame() {
            if (_manager != null) {
                _frame[0] = new Texture2D(_manager.GraphicsDevice, 1, Height);
                Color[] cd0 = new Color[Height];
                for (int i = 0; i < Height; i++) {
                    if (i == 0 || i == Height - 1)
                        cd0[i] = Style.BackgroundColor;
                    else
                        cd0[i] = Color.White;
                }
                _frame[0].SetData(cd0);
                
                // Should only need a 1px texture for horizontal borders.
                // We will just draw them across the top and bottom.
                _frame[1] = new Texture2D(_manager.GraphicsDevice, 1, 1);
                _frame[1].SetData(new Color[] { Color.White });
            }
        }


        public override void OnMove() {
            // Text Positioning
            // It's all left for now.
            //if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    X + Style.PaddingLeft,
                    Y + Style.PaddingTop
                );
            //}
            base.OnMove();
        }


        public override void OnResize() {
            initFrame();
            base.OnResize();
        }


        public void OnTextChanged() {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler TextChanged;


        public override void Update() {
            _oldstate = _keystate;
            _keystate = Keyboard.GetState();

            bool isShiftDown = (!_keystate.CapsLock && (_keystate.IsKeyDown(Keys.LeftShift) || _keystate.IsKeyDown(Keys.RightShift)))
                            || (_keystate.CapsLock && !(_keystate.IsKeyDown(Keys.LeftShift) || _keystate.IsKeyDown(Keys.RightShift)));

            // Use enter to submit, as Style.SubmitButton might be a valid char.
            // No need to do anything here except close, as Text value can be retrieved via property.
            if (isKeyPress(Keys.Enter)) {
                OnSubmit();
                Close();
            }

            // Only control char we need is Backspace, as we don't allow cursor movement.
            if (isKeyPress(Keys.Back)) {
                if (TextLength > 0)
                    Text = Text.Remove(TextLength - 1, 1);
            }

            else {
                // Handle character input.
                foreach (Keys kee in _keystate.GetPressedKeys()) {
                    string k = convertToCharString(kee, isShiftDown);

                    if (TextLength >= MaxLength)
                        break;

                    else if (charTest(k) && isKeyPress(kee)) {
                        if (isAlpha(k) && !isShiftDown)
                            k = k.ToLower();

                        Text = Text + k;
                        OnTextChanged();
                        break;
                    }
                }
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);

            // Draw frame, not style border!
            batch.Draw(_frame[0], new Vector2(X, Y), Style.BorderColor);
            batch.Draw(_frame[1], new Rectangle(X + 1, Y, Width - 2, 1), Style.BorderColor);
            batch.Draw(_frame[1], new Rectangle(X + 1, Y + Height - 1, Width - 2, 1), Style.BorderColor);
            batch.Draw(_frame[0], new Vector2(X + Width - 1, Y), Style.BorderColor);

            batch.DrawString(Style.Font, Text, textPosition, Style.TextColor);
        }


        public bool isKeyPress(Keys kee) {
            return (_keystate.IsKeyDown(kee) && !_oldstate.IsKeyDown(kee));
        }


        private bool isAlpha(string s) {
            return new Regex(@"^[a-zA-Z]$").IsMatch(s);
        }
        private bool isNumeric(string s) {
            return new Regex(@"^[0-9]$").IsMatch(s);
        }
        private bool isAlphaNumeric(string s) {
            return new Regex(@"^[a-zA-Z0-9]$").IsMatch(s);
        }


        private bool charTest(string s) {
            if (InputRules == CharsAllowed.Alpha)
                return isAlpha(s);
            else if (InputRules == CharsAllowed.Numeric)
                return isNumeric(s);
            else if (InputRules == CharsAllowed.AlphaNumeric)
                return isAlphaNumeric(s);
            else
                return new Regex(@"^.$").IsMatch(s);
        }


        private string convertToCharString(Keys kee, bool shift) {
            string k = kee.ToString();

            // Alphabet
            if (isAlpha(k)) {
                if (shift) return k.ToUpper();
                else return k.ToLower();
            }

            // Numbers
            if (new Regex("^D[0-9]$").IsMatch(k)) {
                if (shift) {
                    if (k == "D1") return "!";
                    else if (k == "D2") return "@";
                    else if (k == "D3") return "#";
                    else if (k == "D4") return "$";
                    else if (k == "D5") return "%";
                    else if (k == "D6") return "^";
                    else if (k == "D7") return "&";
                    else if (k == "D8") return "*";
                    else if (k == "D9") return "(";
                    else if (k == "D0") return ")";
                    else return k.Remove(0, 1);
                }

                return k.Remove(0, 1);
            }

            // NumPad
            else if (k.StartsWith("NumPad"))
                return k.Remove(0, 6);
            else if (kee == Keys.Add)
                return "+";
            else if (kee == Keys.Subtract)
                return "-";
            else if (kee == Keys.Multiply)
                return "*";
            else if (kee == Keys.Divide)
                return "/";
            else if (kee == Keys.Decimal)
                return ".";

            // Special Chars
            else if (kee == Keys.OemComma && shift)
                return "<";
            else if (kee == Keys.OemComma && !shift)
                return ",";
            else if (kee == Keys.OemMinus && shift)
                return "_";
            else if (kee == Keys.OemMinus && !shift)
                return "-";
            else if (kee == Keys.OemPeriod && shift)
                return ">";
            else if (kee == Keys.OemPeriod && !shift)
                return ".";
            else if (kee == Keys.OemPlus && shift)
                return "+";
            else if (kee == Keys.OemPlus && !shift)
                return "=";
            else if (kee == Keys.OemQuestion && shift)
                return "?";
            else if (kee == Keys.OemQuestion && !shift)
                return "/";
            else if (kee == Keys.OemSemicolon && shift)
                return ":";
            else if (kee == Keys.OemSemicolon && !shift)
                return ";";
            else if (kee == Keys.OemTilde && shift)
                return "~";
            else if (kee == Keys.OemTilde && !shift)
                return "`";
            else if (kee == Keys.Space)
                return " ";

            // Not included: Keys.OemBackslash, Keys.OemOpenBrackets, 
            // Keys.OemCloseBrackets, Keys.OemPipe, Keys.OemQuotes

            else
                return string.Empty;
        }
    }
}
