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
    /// Displays a menu as a gallery-like two-dimensional grid.
    /// </summary>
    public class GalleryMenu : OdgeControl {
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

        private int _columns;
        public int Columns {
            get { return _columns; }
            set {
                _columns = value;
                repositionOptions();
                Width = Width;
            }
        }


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
                int colcount = 0;
                int tempW = 0;
                int W = 0;

                foreach (AbstractMenuOption option in Options) {
                    tempW += option.Width;
                    colcount++;

                    if (colcount >= Columns) {
                        if (tempW > W)
                            W = tempW;
                        tempW = 0;
                        colcount = 0;
                    }
                }

                return W + Style.PaddingLeft + Style.PaddingRight + (Style.SpacingH * (Columns - 1));
            }
        }


        public GalleryMenu(StyleSheet style, int columns, List<AbstractMenuOption> options, Rectangle area) 
            : base(style) {
            _columns = columns;
            Options = options;
            CascadeStyle();

            // Initialize option dimensions, then cascade real Dimension down.
            repositionOptions();
            Dimensions = area;
        }


        public override void OnOpened() {
            createOptionPanel();

            panelBatch = new SpriteBatch(_manager.GraphicsDevice);
            foreach (AbstractMenuOption option in Options)
                option.OnOpened();

            base.OnOpened();
        }


        public override void OnStyleSet() {
            if (Options != null) {
                repositionOptions();
                Width = Width;
            }
            base.OnStyleSet();
        }


        public override void OnMove() {
            panelRect.X = X + Style.PaddingLeft;
            panelRect.Y = Y + Style.PaddingTop;
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
            Dimensions = new Rectangle(X, Y, Width, Height);
            repositionOptions();
        }

        public void RemoveOption(AbstractMenuOption option) {
            if (SelectedOption == option)
                SelectedIndex--;

            Options.Remove(option);
            repositionOptions();
        }


        /// <summary>
        /// Cascades GalleryMenu's StyleSheet to AbstractListMenuOptions.
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


        /// <summary>
        /// Returns the width of the widest AbstractMenuOption.
        /// </summary>
        /// <returns>int width.</returns>
        public int GetMaxItemWidth() {
            int w = 0;
            foreach (AbstractMenuOption option in Options) {
                if (option.Width > w)
                    w = option.Width;
            }
            return w;
        }


        /// <summary>
        /// Set all option widths at once. 
        /// Use a low value, like 0, to set all options to their minimum widths.
        /// </summary>
        /// <param name="width">int Width for all options.</param>
        public void StandardizeItemWidths(int width) {
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

            repositionOptions();
            Width = Width;
        }


        private void createOptionPanel() {
            if (_manager != null) {
                panelRect = new Rectangle(
                    X + Style.PaddingLeft,
                    Y + Style.PaddingTop,
                    Width - Style.PaddingLeft - Style.PaddingRight,
                    Height - Style.PaddingTop - Style.PaddingBottom
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

            // Right Button.
            else if (OdgeInput.RIGHT) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex + 1 >= Options.Count)
                    SelectedIndex = 0;
                else
                    SelectedIndex++;
                Options[SelectedIndex].OnSelected();
            }

            // Left Button.
            else if (OdgeInput.LEFT) {
                Options[SelectedIndex].OnUnselected();
                if (SelectedIndex - 1 < 0)
                    SelectedIndex = Options.Count - 1;
                else
                    SelectedIndex--;
                Options[SelectedIndex].OnSelected();
            }

            // Down Button.
            else if (OdgeInput.DOWN) {
                int x = SelectedIndex;
                SelectedIndex += Columns;
                if (x != SelectedIndex) {
                    Options[x].OnUnselected();
                    Options[SelectedIndex].OnSelected();
                }
            }

            // Up Button.
            else if (OdgeInput.UP) {
                int x = SelectedIndex;
                SelectedIndex -= Columns;
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


        private void repositionOptions() {
            int x = 0;
            int y = 0;
            int rowheight = 0;
            int count = 0;
            
            foreach (AbstractMenuOption option in Options){
                option.Dimensions = new Rectangle(x, y, option.Width, option.Height);
                x += option.Width + Style.SpacingH;
                if (rowheight < option.Height)
                    rowheight = option.Height;
                count++;

                if (count == Columns){
                    x = 0;
                    y += rowheight + Style.SpacingV;
                    rowheight = 0;
                    count = 0;
                }
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
