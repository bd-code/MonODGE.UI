using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    /// <summary>
    /// Displays a scrollable list of options extended from AbstractListMenuOption.
    /// </summary>
    public class ListMenu : OdgeControl {
        private string title;
        private Vector2 textPosition;
        private Vector2 textDimensions;

        public List<AbstractMenuOption> Options { get; protected set; }

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

        public AbstractMenuOption SelectedOption {
            get {
                if (Options != null && Options.Count > 0)
                    return Options[SelectedIndex];
                else
                    return null;
            }
        }

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
                base.Dimensions = value;
                
                if (_manager != null)
                    createOptionPanel();
            }
        }


        protected override int MinWidth {
            get {
                int mw = Style.PaddingLeft + Style.PaddingRight;
                if (Options.Count > 0)
                    mw += Options[0].Width;
                return mw;
            }
        }


        public ListMenu(StyleSheet style, string heading, Rectangle area) : 
            this(style, heading, new List<AbstractMenuOption>(), area) { }

        public ListMenu(StyleSheet style, string heading, List<AbstractMenuOption> listOptions, Rectangle area) 
            : base(style) {
            // Set options and options' style first.
            Options = listOptions;            
            CascadeStyle();

            // Need textDimensions before we set the menu and option Dimensions.
            title = heading;
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);

            // Initialize option dimensions, then cascade real Dimension down.
            resizeOptions(area.Width - Style.PaddingLeft - Style.PaddingRight);
            repositionOptions();
            Dimensions = area;
            if (area.Location == Point.Zero)
                repositionText();
        }


        public override void OnOpened() {
            createOptionPanel();

            panelBatch = new SpriteBatch(_manager.GraphicsDevice);
            foreach (AbstractMenuOption option in Options)
                option.OnOpened();

            base.OnOpened();
        }


        public override void OnStyleSet() {
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);
            if (Options != null)
                Dimensions = new Rectangle(X, Y, Width, Height);
            base.OnStyleSet();
        }


        public override void OnMove() {
            repositionText();
            panelRect.X = X + Style.PaddingLeft;
            panelRect.Y = Y + (int)textDimensions.Y + Style.PaddingTop;
            base.OnMove();
        }

        /// <summary>
        /// This is called in Update when Options.Count == 0.
        /// </summary>
        public virtual void OnEmpty() { Emptied?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Emptied;


        public override void Update() {
            if (Options.Count > 0) {
                handleInput();

                // Scroll if Selection Offscreen
                scrollOptions();

                // Update each option.
                for (int p = 0; p < Options.Count; p++) {
                    if (p == SelectedIndex)
                        Options[p].Update(true);
                    else
                        Options[p].Update(false);
                }
            }
            
            else {
                // Cancel.
                if (CheckCancel) {
                    OnCancel();
                    return;
                }
                else {
                    OnEmpty();
                }
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            if (title != null)
                batch.DrawString(Style.HeaderFont, title, textPosition, Style.HeaderColor);

            // Draw option panel.
            if (Options.Count > 0 && optionPanel != null) {
                DrawToPanel();
                batch.Draw(optionPanel, panelRect, Color.White);
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


        public void AddOption(AbstractMenuOption option) {
            option.Style = Style;
            Options.Add(option);
            resizeOptions(Options[0].Width);
            repositionOptions();
        }

        public void RemoveOption(AbstractMenuOption option) {
            if (SelectedOption == option)
                SelectedIndex--;
            
            Options.Remove(option);
            repositionOptions();
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


        private void createOptionPanel() {
            if (_manager != null) {
                panelRect = new Rectangle(
                    X + Style.PaddingLeft,
                    Y + (int)textDimensions.Y + Style.PaddingTop + Style.PaddingBottom,
                    Width - Style.PaddingLeft - Style.PaddingRight,
                    Height - (int)textDimensions.Y - Style.PaddingTop - Style.PaddingBottom
                    );

                optionPanel = _manager.CreateRenderTarget(panelRect.Width, panelRect.Height);
            }
            else {
                optionPanel = null;
                panelRect = new Rectangle(0, 0, 0, 0);
                throw new NullReferenceException("createOptionPanel must be called in or after OnOpened(), so _manager is not null.");
            }
        }


        private void handleInput() {
            // Submit.
            if (CheckSubmit) {
                Options[SelectedIndex].OnSubmit();
                // this.OnSubmit()? Maybe containers shouldn't have submit.
            }

            // Move Down.
            else if (OdgeInput.DOWN) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex + 1 >= Options.Count)
                    SelectedIndex = 0;
                else
                    SelectedIndex++;
                Options[SelectedIndex].OnSelected();
            }

            // Move Up!
            else if (OdgeInput.UP) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex - 1 < 0)
                    SelectedIndex = Options.Count - 1;
                else
                    SelectedIndex--;
                Options[SelectedIndex].OnSelected();
            }

            // Jump Down.
            else if (OdgeInput.RIGHT) {
                int x = SelectedIndex;
                SelectedIndex += 8;
                if (x != SelectedIndex) {
                    Options[x].OnUnselected();
                    Options[SelectedIndex].OnSelected();
                }
            }

            // Jump Up!
            else if (OdgeInput.LEFT) {
                int x = SelectedIndex;
                SelectedIndex -= 8;
                if (x != SelectedIndex) {
                    Options[x].OnUnselected();
                    Options[SelectedIndex].OnSelected();
                }
            }

            // Cancel.
            else if (CheckCancel) {
                OnCancel();
            }
        }


        private void repositionText() {
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

        }


        private void repositionOptions() {
            int ypos = 0;
            foreach (AbstractMenuOption option in Options) {
                option.Dimensions = new Rectangle(0, ypos, option.Width, option.Height);
                ypos += option.Height + option.Style.SpacingV;
            }
        }


        private void resizeOptions(int width = 0) {
            // Set initial option widths.
            foreach (AbstractMenuOption option in Options) {
                option.Width = width;

                if (option.Width > width)
                    width = option.Width;
            }

            // Resize options again to full width.
            foreach (AbstractMenuOption option in Options) {
                if (option.Width < width)
                    option.Width = width;
            }
        }


        private void scrollOptions() {
            // SelectedOption too high
            if (SelectedOption.Y < 0) {
                int y = -SelectedOption.Y;
                foreach (AbstractMenuOption op in Options)
                    op.Y += (y / 2) + 1;
            }

            // SelectedOption too low
            else if ((SelectedOption.Y + SelectedOption.Height) > (panelRect.Height)) {
                int y = SelectedOption.Y + SelectedOption.Height - panelRect.Height;
                foreach (AbstractMenuOption op in Options)
                    op.Y -= (y / 2) + 1;
            }
        }
    }
}
