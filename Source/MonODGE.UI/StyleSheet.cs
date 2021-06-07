
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI {
    public class StyleSheet : IChangeTracking {
        // Screen dimensions are static for size and consistency.
        public static int ScreenWidth { get; internal set; }
        public static int ScreenHeight { get; internal set; }
        public bool IsChanged { get; private set; }

        //// Alignment ////

        public enum AlignmentsH { LEFT, CENTER, RIGHT }
        public enum AlignmentsV { TOP, CENTER, BOTTOM }

        /// <summary>
        /// Horizontal alignment for inner text and elements.
        /// </summary>
        public AlignmentsH TextAlignH {
            get { return _textAlignH; }
            set {
                _textAlignH = value;
                IsChanged = true;
            }
        }
        private AlignmentsH _textAlignH;
        
        /// <summary>
        /// Vertical alignment for inner text and elements.
        /// </summary>
        public AlignmentsV TextAlignV {
            get { return _textAlignV; }
            set {
                _textAlignV = value;
                IsChanged = true;
            }
        }
        private AlignmentsV _textAlignV;
        
        
        //// Background ////

        /// <summary>
        /// Background texture which fills Component dimensions. Can be set to null for no background.
        /// </summary>
        public Texture2D Background {
            get { return _background; }
            set {
                _background = value;
                IsChanged = true;
            }
        }
        private Texture2D _background;

        /// <summary>
        /// Border texture color. Set to Color.White to match original image.
        /// </summary>
        public Color BackgroundColor {
            get { return _bgColor; }
            set {
                _bgColor = value;
                IsChanged = true;
            }
        }
        private Color _bgColor;
        

        //// Borders ////

        private Texture2D _borders;

        /// <summary>
        /// Border texture which, uh, borders Components. Can be set to null for no borders.
        /// Border textures are split into 3*3 tiles, and thus image dimensions should be divisible by 3.
        /// </summary>
        public Texture2D Borders {
            get { return _borders; }
            set {
                _borders = value;
                BorderSourceRects = new Rectangle[] {
                    new Rectangle(0, 0, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, 0, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, 0, BorderTileWidth, BorderTileHeight),

                    new Rectangle(0, BorderTileHeight, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, BorderTileHeight, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, BorderTileHeight, BorderTileWidth, BorderTileHeight),

                    new Rectangle(0, BorderTileHeight*2, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth, BorderTileHeight*2, BorderTileWidth, BorderTileHeight),
                    new Rectangle(BorderTileWidth*2, BorderTileHeight*2, BorderTileWidth, BorderTileHeight)
                };
                IsChanged = true;
            }
        }

        /// <summary>
        /// 0-UpperLeft, 1-UpperCenter, 2-UpperRight, 
        /// 3-MiddleLeft, 4-Middle[Unused], 5-MiddleRight,
        /// 6-LowerLeft, 7-LowerCenter, 8-LowerRight
        /// </summary>
        public Rectangle[] BorderSourceRects { get; private set; }

        /// <summary>
        /// Width of each border tile, or 0 if Borders is null.
        /// </summary>
        public int BorderTileWidth { get { return _borders?.Width / 3 ?? 0; } }

        /// <summary>
        /// Height of each border tile, or 0 if Borders is null.
        /// </summary>
        public int BorderTileHeight { get { return _borders?.Height / 3 ?? 0; } }

        /// <summary>
        /// Border texture color. Set to Color.White to match original image.
        /// </summary>
        public Color BorderColor {
            get { return _borderColor; }
            set {
                _borderColor = value;
                IsChanged = true;
            }
        }
        private Color _borderColor;


        //// Fonts and Text Color ////

        /// <summary>
        /// Header text font.
        /// </summary>
        public SpriteFont HeaderFont {
            get { return _headerFont; }
            set {
                _headerFont = value;
                IsChanged = true;
            }
        }
        private SpriteFont _headerFont;

        /// <summary>
        /// Header text color.
        /// </summary>
        public Color HeaderColor {
            get { return _headerColor; }
            set {
                _headerColor = value;
                IsChanged = true;
            }
        }
        private Color _headerColor;

        /// <summary>
        /// Main text font.
        /// </summary>
        public SpriteFont Font {
            get { return _font; }
            set {
                _font = value;
                if (_headerFont == null)
                    _headerFont = value;
                if (_footerFont == null)
                    _footerFont = value;
                IsChanged = true;
            }
        }
        private SpriteFont _font;

        /// <summary>
        /// Main text color.
        /// </summary>
        public Color TextColor {
            get { return _textColor; }
            set {
                _textColor = value;
                if (_headerColor == null)
                    _headerColor = value;
                if (_footerColor == null)
                    _footerColor = value;
                IsChanged = true;
            }
        }
        private Color _textColor;

        /// <summary>
        /// Footer text font.
        /// </summary>
        public SpriteFont FooterFont {
            get { return _footerFont; }
            set {
                _footerFont = value;
                IsChanged = true;
            }
        }
        private SpriteFont _footerFont;

        /// <summary>
        /// Footer text color.
        /// </summary>
        public Color FooterColor {
            get { return _footerColor; }
            set {
                _footerColor = value;
                IsChanged = true;
            }
        }
        private Color _footerColor;
        
        /// <summary>
        /// For menus and other controls, selected item text is displayed in this color rather than TextColor.
        /// </summary>
        public Color SelectedTextColor {
            get { return _selectedColor; }
            set {
                _selectedColor = value;
                IsChanged = true;
            }
        }
        private Color _selectedColor;

        /// <summary>
        /// For menus and other controls, unselected item text is displayed in this color rather than TextColor.
        /// </summary>
        public Color UnselectedTextColor {
            get { return _unselectedColor; }
            set {
                _unselectedColor = value;
                IsChanged = true;
            }
        }
        private Color _unselectedColor;


        //// Text Shadows ////

        /*/// <summary>
        /// Pixel size of shadow/outline around text.
        /// </summary>
        public int TextShadowSize {
            get { return _textShadowSize; }
            set {
                _textShadowSize = value;
                IsChanged = true;
            }
        }
        private int _textShadowSize;*/


        /// <summary>
        /// A Vector[4] array that determines the position of the four text shadows. 
        /// Padding order: 0-TopLeft, 1-TopRight, 2-BottomLeft, 3-BottomRight.
        /// </summary>
        public Vector2[] TextShadows {
            get { return _textShadows; }
            set {
                if (value.Length >= 4) {
                    _textShadows[0] = value[0];
                    _textShadows[1] = value[1];
                    _textShadows[2] = value[2];
                    _textShadows[3] = value[3];
                }
                else if (value.Length == 3) {
                    _textShadows[0] = value[0];
                    _textShadows[1] = _textShadows[3] = value[1];
                    _textShadows[2] = value[2];
                }
                else if (value.Length == 2) {
                    _textShadows[0] = _textShadows[2] = value[0];
                    _textShadows[1] = _textShadows[3] = value[1];
                }
                else if (value.Length == 1) {
                    _textShadows[0] = _textShadows[1] = _textShadows[2] = _textShadows[3] = value[0];
                }
                IsChanged = true;
            }
        }
        private Vector2[] _textShadows;

        /// <summary>
        /// Sets top-left text shadow position.
        /// </summary>
        public Vector2 TextShadowTopLeft {
            get { return _textShadows[0]; }
            set {
                _textShadows[0] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets top-right text shadow position.
        /// </summary>
        public Vector2 TextShadowTopRight {
            get { return _textShadows[1]; }
            set {
                _textShadows[1] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets bottom-left text shadow position.
        /// </summary>
        public Vector2 TextShadowBottomLeft {
            get { return _textShadows[2]; }
            set {
                _textShadows[2] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets bottom-right text shadow position.
        /// </summary>
        public Vector2 TextShadowBottomRight {
            get { return _textShadows[3]; }
            set {
                _textShadows[3] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Text outline color.
        /// </summary>
        public Color TextShadowColor {
            get { return _textShadowColor; }
            set {
                _textShadowColor = value;
                IsChanged = true;
            }
        }
        private Color _textShadowColor;

        /// <summary>
        /// TextShadow Preset: Adds a 1px "glow" around text. Great for readability.
        /// </summary>
        public static Vector2[] TextShadow_1pxGlow {
            get { return new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) }; }
        }


        //// Padding ////

        /// <summary>
        /// An int[4] array that determines the Component's inner padding. 
        /// Padding order: 0-Top, 1-Right, 2-Bottom, 3-Left.
        /// </summary>
        public int[] Padding {
            get { return _padding; }
            set {
                if (value.Length >= 4) {
                    _padding[0] = value[0];
                    _padding[1] = value[1];
                    _padding[2] = value[2];
                    _padding[3] = value[3];
                }
                else if (value.Length == 3) {
                    _padding[0] = value[0];
                    _padding[1] = _padding[3] = value[1];
                    _padding[2] = value[2];
                }
                else if (value.Length == 2) {
                    _padding[0] = _padding[2] = value[0];
                    _padding[1] = _padding[3] = value[1];
                }
                else if (value.Length == 1) {
                    _padding[0] = _padding[1] = _padding[2] = _padding[3] = value[0];
                }
                IsChanged = true;
            }
        }
        private int[] _padding;

        /// <summary>
        /// Use this to set the inner padding on all four sides at once.
        /// </summary>
        public int PaddingAll {
            set {
                _padding[0] = _padding[1] = _padding[2] = _padding[3] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets top inner padding.
        /// </summary>
        public int PaddingTop {
            get { return _padding[0]; }
            set {
                _padding[0] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets right inner padding.
        /// </summary>
        public int PaddingRight {
            get { return _padding[1]; }
            set {
                _padding[1] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets bottom inner padding.
        /// </summary>
        public int PaddingBottom {
            get { return _padding[2]; }
            set {
                _padding[2] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets left inner padding.
        /// </summary>
        public int PaddingLeft {
            get { return _padding[3]; }
            set {
                _padding[3] = value;
                IsChanged = true;
            }
        }


        //// Spacing ////

        /// <summary>
        /// An int[2] array. Determines distance between child Components in OdgeComponents that have them.
        /// Padding order: 0-Horizontal, 1-Vertical
        /// </summary>
        public int[] Spacing {
            get { return _spacing; }
            set {
                if (value.Length >= 2) {
                    _spacing[0] = value[0];
                    _spacing[1] = value[1];
                }
                else if (value.Length == 1) {
                    _spacing[0] = _spacing[1] = value[0];
                }
                IsChanged = true;
            }
        }
        private int[] _spacing;

        /// <summary>
        /// Use this to set both horizontal and vertical spacing at once.
        /// </summary>
        public int SpacingAll {
            set {
                _spacing[0] = _spacing[1] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets horizontal spacing.
        /// </summary>
        public int SpacingH {
            get { return _spacing[0]; }
            set {
                _spacing[0] = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Sets vertical spacing.
        /// </summary>
        public int SpacingV {
            get { return _spacing[1]; }
            set {
                _spacing[1] = value;
                IsChanged = true;
            }
        }


        //// Input Mapping ////

        /// <summary>
        /// This Keyboard Key triggers the Component's OnSubmit().
        /// </summary>
        public Keys SubmitKey {
            get { return _submitKey; }
            set {
                _submitKey = value;
                IsChanged = true;
            }
        }
        private Keys _submitKey;

        /// <summary>
        /// This GamePad Button triggers the Component's OnSubmit().
        /// </summary>
        public Buttons SubmitButton {
            get { return _submitButton; }
            set {
                _submitButton = value;
                IsChanged = true;
            }
        }
        private Buttons _submitButton;

        /// <summary>
        /// This Keyboard Key triggers the Component's OnCancel(). 
        /// For most Controls, it also closes it.
        /// </summary>
        public Keys CancelKey {
            get { return _cancelKey; }
            set {
                _cancelKey = value;
                IsChanged = true;
            }
        }
        private Keys _cancelKey;

        /// <summary>
        /// This GamePad Button triggers the Component's OnCancel(). 
        /// For most Controls, it also closes it.
        /// </summary>
        public Buttons CancelButton {
            get { return _cancelButton; }
            set {
                _cancelButton = value;
                IsChanged = true;
            }
        }
        private Buttons _cancelButton;

        /// <summary>
        /// If true, Control is closed upon upon pressing CancelKey.
        /// </summary>
        public bool CloseOnCancel { get; set; } // No IsChanged necessary.


        public StyleSheet(int screenwidth, int screenheight) {
            ScreenWidth = screenwidth;
            ScreenHeight = screenheight;
            BackgroundColor = Color.TransparentBlack;            
            BorderColor = Color.White;

            TextColor = Color.White;
            SelectedTextColor = Color.Gold;
            UnselectedTextColor = Color.Gray;
            
            TextAlignH = AlignmentsH.LEFT;
            TextAlignV = AlignmentsV.TOP;

            _padding = new int[4] { 0, 0, 0, 0 };
            _spacing = new int[2] { 0, 0 };
            _textShadows = new Vector2[4] {
                Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero
            };

            IsChanged = false;
        }


        /// <summary>
        /// Creates a clone of this StyleSheet.
        /// </summary>
        /// <returns>A new StyleSheet object with this StyleSheet's values.</returns>
        public StyleSheet Clone() {
            StyleSheet clone = new StyleSheet(ScreenWidth, ScreenHeight);
            clone.Background = Background;              clone.BackgroundColor = BackgroundColor;
            clone.BorderColor = BorderColor;            clone.Borders = Borders;
            clone.BorderSourceRects = BorderSourceRects;
            clone.CancelButton = CancelButton;          clone.CancelKey = CancelKey;
            clone.CloseOnCancel = CloseOnCancel;
            clone.Font = Font;
            clone.FooterColor = FooterColor;            clone.FooterFont = FooterFont;
            clone.HeaderColor = HeaderColor;            clone.HeaderFont = HeaderFont;
            clone.Padding = Padding;
            clone.SelectedTextColor = SelectedTextColor;    clone.UnselectedTextColor = UnselectedTextColor;
            clone.Spacing = Spacing;
            clone.SubmitButton = SubmitButton;          clone.SubmitKey = SubmitKey;
            clone.TextAlignH = TextAlignH;              clone.TextAlignV = TextAlignV;
            clone.TextColor = TextColor;
            clone.TextShadowColor = TextShadowColor;    clone.TextShadows = TextShadows;
            clone.IsChanged = false;
            return clone;
        }

        public void RegisterChanges() { IsChanged = true; }
        public void AcceptChanges() { IsChanged = false; }
    }
}
