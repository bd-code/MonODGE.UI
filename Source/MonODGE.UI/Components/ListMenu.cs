using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Components {
    /// <summary>
    /// Vertically displays a list of options extended from AbstractListMenuOption.
    /// </summary>
    public class ListMenu : OdgeControl {
        /*
        Needs Add(option) and Remove(option) for dynamic lists (think inventory).
        Re-design with title and scrolling ListOptionPanel.
        */
        private string title;
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public List<AbstractMenuOption> Options { get; private set; }
        private int _selectedIndex;
        public int SelectedIndex {
            get { return _selectedIndex; }
            set {
                value = MathHelper.Clamp(value, 0, Options.Count - 1);
                if (_selectedIndex != value) {
                    SelectedOption.OnUnselected();
                    _selectedIndex = value;
                    SelectedOption.OnSelected();
                }
            }
        }

        public AbstractMenuOption SelectedOption { get { return Options[SelectedIndex]; } }
        
        private RenderTarget2D optionPanel;
        private Rectangle panelRect;
        private SpriteBatch panelBatch;

        public override StyleSheet Style {
            get { return base.Style; }
            set {
                base.Style = value;
                CascadeStyle();
            }
        }

        public override Rectangle Dimensions {
            get { return base.Dimensions; }
            set {
                int optionWidth = value.Width - Style.PaddingLeft - Style.PaddingRight;

                // Set initial option widths.
                foreach (AbstractMenuOption option in Options) {
                    option.Width = optionWidth;

                    if (option.Width > optionWidth)
                        optionWidth = option.Width;
                }

                // Resize options again to full width.
                foreach (AbstractMenuOption option in Options) {
                    if (option.Width < optionWidth)
                        option.Width = optionWidth;
                }

                value.Width = optionWidth + Style.PaddingLeft + Style.PaddingRight;
                base.Dimensions = value;
                
                createOptionPanel();
            }
        }


        public ListMenu(StyleSheet style, string heading, Rectangle area, bool canCancel = true) : 
            this(style, heading, new List<AbstractMenuOption>(), area, canCancel) { }

        public ListMenu(StyleSheet style, string heading, List<AbstractMenuOption> listOptions, Rectangle area, bool canCancel = true) 
            : base(style) {
            // Set local privates first.
            Options = listOptions;            
            CascadeStyle();

            // Need textDimensions before we set the menu and option Dimensions.
            title = heading;
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);

            // Initialize option dimensions, then cascade real Dimension down.
            foreach (AbstractMenuOption option in listOptions)
                option.Dimensions = new Rectangle(-1, -1, 0, 0);

            Dimensions = area;
            IsCancelable = canCancel;
        }


        public override void Initialize() {
            createOptionPanel();

            panelBatch = new SpriteBatch(_manager.GraphicsDevice);
            foreach (AbstractMenuOption option in Options)
                option.Initialize();
        }


        public override void OnStyleSet() {
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);
            // Reset Dimensions by simply setting same values. Property will take care of resizing.
            // BUT this is broken because OnSetStyle is called before Options are a thing.
            //Dimensions = new Rectangle(X, Y, Width, Height);
        }


        public override void OnMove() {
            // Text Positioning
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT) {
                textPosition = new Vector2(
                    X + Style.PaddingLeft,
                    Y + Style.PaddingTop
                );
            }
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER) {
                textPosition = new Vector2(
                    Dimensions.Center.X - (textDimensions.X / 2),
                    Y + Style.PaddingTop
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Dimensions.Right - textDimensions.X - Style.PaddingRight,
                    Y + Style.PaddingTop
                );
            }

            // Reposition options. Is this necessary?
            int ypos = 0;
            foreach (AbstractMenuOption option in Options) {
                option.Dimensions = new Rectangle(0, ypos, option.Width, option.Height);
                ypos += option.Height;
            }
        }


        public override void OnCancel() {
            if (IsCancelable)
                Close();
        }


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.CancelKey) && IsCancelable) {
                Close();
            }
            
            // Handle input.
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                Options[SelectedIndex].OnSubmit();
                // this.OnSubmit()? Maybe containers shouldn't have submit.
            }

            // Move Down.
            else if (_manager.Input.isKeyPress(Keys.Down) || _manager.Input.isKeyPress(Keys.S)) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex + 1 >= Options.Count)
                    SelectedIndex = 0;
                else
                    SelectedIndex++;
                Options[SelectedIndex].OnSelected();
            }

            // Move Up!
            else if (_manager.Input.isKeyPress(Keys.Up) || _manager.Input.isKeyPress(Keys.W)) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex - 1 < 0)
                    SelectedIndex = Options.Count - 1;
                else
                    SelectedIndex--;
                Options[SelectedIndex].OnSelected();
            }

            else if (_manager.Input.isKeyPress(Style.CancelKey)) {
                OnCancel();
            }
        }


        private void DrawToPanel() {
            _manager.GraphicsDevice.SetRenderTarget(optionPanel);
            _manager.GraphicsDevice.Clear(Color.TransparentBlack);
            panelBatch.Begin();

            for (int p = 0; p < Options.Count; p++) {
                if (p == SelectedIndex)
                    Options[p].Draw(panelBatch, true);
                else
                    Options[p].Draw(panelBatch, false);
            }

            panelBatch.End();
            _manager.GraphicsDevice.SetRenderTarget(null);
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            if (title != null)
                batch.DrawString(Style.HeaderFont, title, textPosition, Style.HeaderColor);

            // Draw option panel.
            if (optionPanel != null) {
                DrawToPanel();
                batch.Draw(optionPanel, panelRect, Color.White);
            }
        }


        private void createOptionPanel() {
            if (_manager != null) {
                optionPanel = _manager.CreateRenderTarget(
                    Width - Style.PaddingLeft - Style.PaddingRight,
                    Height - (int)textDimensions.Y - Style.PaddingTop - Style.PaddingBottom
                    );

                panelRect = new Rectangle(
                    X + Style.PaddingLeft,
                    Y + (int)textDimensions.Y + Style.PaddingTop,
                    Width - Style.PaddingLeft - Style.PaddingRight,
                    Height - (int)textDimensions.Y - Style.PaddingTop - Style.PaddingBottom
                    );
            }
            else {
                optionPanel = null;
                panelRect = new Rectangle(0, 0, 0, 0);
                //throw new NullReferenceException("createOptionPanel must be called in or after Initialize(), so _manager is not null.");
            }
        }


        /// <summary>
        /// Cascades ListMenu's StyleSheet to AbstractListMenuOptions.
        /// </summary>
        /// <param name="forced">
        /// If true, sets option's StyleSheet even if the option already has one.
        /// If false (default), only sets option's StyleSheet if it is null.
        /// </param>
        public void CascadeStyle(bool forced = false) {
            if (Options != null) {
                foreach (AbstractMenuOption option in Options)
                    if (forced || option.Style == null)
                        option.Style = Style;
            }
        }
    }
}
