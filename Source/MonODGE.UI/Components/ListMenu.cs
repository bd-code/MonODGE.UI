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

        public override StyleSheet Style {
            get { return base.Style; }
            set {
                base.Style = value;
                CascadeStyle();
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
        

        protected int OptionTopBound { get { return Y + Style.PaddingTop + (int)textDimensions.Y + Style.PaddingBottom; } }
        protected int OptionLowBound { get { return Dimensions.Bottom - Style.PaddingBottom; } }


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
            if (title != null)
                batch.DrawString(Style.HeaderFont, title, textPosition, Style.HeaderColor);

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
            int xpos = X + Style.PaddingLeft;
            int ypos = OptionTopBound;
            foreach (AbstractMenuOption option in Options) {
                option.Dimensions = new Rectangle(xpos, ypos, option.Width, option.Height);
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
            int topBound = OptionTopBound;
            int lowBound = OptionLowBound;

            // SelectedOption too high
            if (SelectedOption.Y < topBound) {
                int dy = -(SelectedOption.Y - topBound);
                foreach (AbstractMenuOption op in Options)
                    op.Y += (dy / 2) + 1;
            }

            // SelectedOption too low
            else if (SelectedOption.Dimensions.Bottom > lowBound) {
                int dy = SelectedOption.Dimensions.Bottom - lowBound;
                foreach (AbstractMenuOption op in Options)
                    op.Y -= (dy / 2) + 1;
            }
        }
    }
}
