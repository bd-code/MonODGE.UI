using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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


        protected int OptionTopBound { get { return Y + Style.PaddingTop; } }
        protected int OptionLowBound { get { return Dimensions.Bottom - Style.PaddingBottom; } }


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
            repositionOptions();
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

            // Draw Options
            for (int p = 0; p < Options.Count; p++) {
                if (Options[p].Y >= OptionTopBound && Options[p].Dimensions.Bottom <= OptionLowBound) {
                    if (p == SelectedIndex)
                        Options[p].Draw(batch, true);
                    else
                        Options[p].Draw(batch, false);
                }
            }
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
            int x = X + Style.PaddingLeft;
            int y = OptionTopBound;
            int rowheight = 0;
            int count = 0;
            
            foreach (AbstractMenuOption option in Options){
                option.Dimensions = new Rectangle(x, y, option.Width, option.Height);
                x += option.Width + Style.SpacingH;
                if (rowheight < option.Height)
                    rowheight = option.Height;
                count++;

                if (count == Columns){
                    x = X + Style.PaddingLeft;
                    y += rowheight + Style.SpacingV;
                    rowheight = 0;
                    count = 0;
                }
            }
        }


        private void scrollOptions() {
            int topBound = OptionTopBound;
            int lowBound = OptionLowBound;
            int correction = 0;

            // SelectedOption too high
            if (SelectedOption.Y < topBound) {
                int dy = -(SelectedOption.Y - topBound);
                foreach (AbstractMenuOption op in Options)
                    op.Y += (dy / 2) + 1;
            }

            // SelectedOption too low
            else if (SelectedOption.Dimensions.Bottom > lowBound) {
                int dy = SelectedOption.Dimensions.Bottom - lowBound;
                SelectedOption.Y -= (dy / 2) + 1;
                foreach (AbstractMenuOption op in Options) {
                    if (op == SelectedOption)
                        continue;

                    op.Y -= (dy / 2) + 1;
                    if (op.Y == SelectedOption.Y && op.Dimensions.Bottom > lowBound && SelectedOption.Dimensions.Bottom <= lowBound)
                        correction = MathHelper.Max(op.Dimensions.Bottom - lowBound, correction);
                }
            }

            // Correct if bottom row extends past lowBound.
            if (correction > 0) {
                foreach (AbstractMenuOption op in Options) 
                    op.Y -= correction;                
            }
        }
    }
}
