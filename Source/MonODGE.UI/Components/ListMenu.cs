using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    /// <summary>
    /// Displays a scrollable list of option buttons extended from OdgeButton.
    /// </summary>
    public class ListMenu : OdgeControl {
        protected StyledText titlebox;
        protected Point textPoint;

        protected List<OdgeButton> Options { get; set; }
        public OdgeButton[] Buttons { get { return Options.ToArray(); } }
        protected Rectangle bpanel;

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

        public OdgeButton SelectedOption {
            get {
                if (Options != null && Options.Count > 0)
                    return Options[SelectedIndex];
                else
                    return null;
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


        public ListMenu(StyleSheet style, Rectangle area, string heading)
            : base(style) {
            Options = new List<OdgeButton>();
            if (string.IsNullOrEmpty(heading)) {
                titlebox = new StyledText(style, "");
                titlebox.Dimensions = Rectangle.Empty;
            }
            else {
                titlebox = new StyledText(style, heading);
                titlebox.StyleMode = StyledText.StyleModes.Header;
            }
            Dimensions = area;
        }


        public override void OnOpened() {
            foreach (OdgeButton option in Options)
                option.OnOpened();

            base.OnOpened();
        }


        public override void OnStyleChanged() {
            titlebox.OnStyleChanged();
            calcTextPoint();
            calcPanel();

            if (Options.Count > 0) {
                foreach (OdgeButton option in Options) {
                    if (option.Style.IsChanged) {
                        option.OnStyleChanged();
                    }
                }
                // Loop twice to avoid skipping shared styles after AcceptChanges().
                foreach (OdgeButton option in Options) {
                    if (option.Style.IsChanged) {
                        option.Style.AcceptChanges();
                    }
                }

                resetBtns(true);
                calcPanel(); // Yes, again.
            }

            base.OnStyleChanged();
        }


        public override void OnMove() {
            calcPanel();
            base.OnMove();
        }
        public override void OnResize() {
            calcTextPoint();
            calcPanel();
            base.OnResize();
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
                    Options[p].Update();
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
            DrawBG(batch);
            DrawBorders(batch);
            //batch.Draw(Style.Background, bpanel, Color.Aquamarine);
            titlebox.Draw(batch, new Rectangle(Dimensions.Location + textPoint, Dimensions.Size));
            
            for (int p = 0; p < Options.Count; p++) {
                if (Options[p].Y >= 0 && Options[p].Dimensions.Bottom <= bpanel.Height) {
                    Options[p].Draw(batch, bpanel);
                }
            }
        }
        public override void Draw(SpriteBatch batch, Rectangle parentRect) {
            Rectangle where = new Rectangle(Dimensions.Location + parentRect.Location, Dimensions.Size);
            DrawBG(batch, where);
            DrawBorders(batch, where);
            titlebox.Draw(batch, new Rectangle(where.Location + textPoint, where.Size));

            Rectangle bp2 = new Rectangle(bpanel.Location + parentRect.Location, bpanel.Size);
            for (int p = 0; p < Options.Count; p++) {
                if (Options[p].Y >= 0 && Options[p].Dimensions.Bottom <= bp2.Height) {
                    Options[p].Draw(batch, bp2);
                }
            }
        }


        public void AddOption(OdgeButton option) {
            Options.Add(option);
            if (Options.Count == 1) {
                option.Y = 0;
                option.OnSelected();
            }
            resetBtns(true);
        }

        public void RemoveOption(OdgeButton option) {
            if (SelectedIndex == Options.Count-1)
                SelectedIndex--;
            
            Options.Remove(option);
            resetBtns(true);
        }


        /// <summary>
        /// Cascades ListMenu's StyleSheet to OdgeButtons.
        /// </summary>
        /// <param name="forced">
        /// If true, sets option's StyleSheet even if the option already has one.
        /// If false (default), only sets option's StyleSheet if it is null.
        /// </param>
        public void CascadeStyle(bool forced = false) {
            foreach (OdgeButton option in Options)
                if (forced || option.Style == null) {
                    option.Style = Style;
                    option.Style.RegisterChanges();
                }
        }


        private void handleInput() {
            // Submit.
            if (CheckSubmit) {
                SelectedOption.OnSubmit();
                // this.OnSubmit()? Maybe containers shouldn't have submit.
            }

            // Move Down.
            else if (OdgeUIInput.DOWN) {
                if (SelectedIndex + 1 >= Options.Count)
                    SelectedIndex = 0;
                else
                    SelectedIndex++;
            }

            // Move Up!
            else if (OdgeUIInput.UP) {
                if (SelectedIndex - 1 < 0)
                    SelectedIndex = Options.Count - 1;
                else
                    SelectedIndex--;
            }

            // Jump Down.
            else if (OdgeUIInput.RIGHT) {
                SelectedIndex += 8;
            }

            // Jump Up!
            else if (OdgeUIInput.LEFT) {
                SelectedIndex -= 8;
            }

            // Cancel.
            else if (CheckCancel) {
                OnCancel();
            }
        }


        private void calcTextPoint() {
            textPoint.Y = Style.PaddingTop;

            // Horizontal
            if (Style.TextAlignH == StyleSheet.AlignmentsH.LEFT)
                textPoint.X = Style.PaddingLeft;
            else if (Style.TextAlignH == StyleSheet.AlignmentsH.CENTER)
                textPoint.X = Width / 2 - (titlebox.Width / 2);
            else // Right
                textPoint.X = Width - titlebox.Width - Style.PaddingRight;
        }


        private void resetBtns(bool fullWidth = true) {
            int wdh = fullWidth ? bpanel.Width : 1;
            int ypos = Options[0]?.Y ?? 0;

            // Get max width.
            foreach (OdgeButton option in Options) {
                wdh = Math.Max(wdh, option.Width);
            }

            // Reposition and resize options to full width.
            foreach (OdgeButton option in Options) {
                option.Dimensions = new Rectangle(0, ypos, wdh, option.Height);
                ypos += option.Height + Style.SpacingV;
            }

            // Since widening options affect MinWidth, check for that too.
            if (Width < MinWidth)
                Width = MinWidth;
        }


        private void calcPanel() {
            bpanel = new Rectangle(
                X + Style.PaddingLeft,
                Y + Style.PaddingTop + titlebox.Height + Style.SpacingV,
                Width - Style.PaddingLeft - Style.PaddingRight,
                Height - (Style.PaddingTop + titlebox.Height + Style.SpacingV + Style.PaddingBottom)
                );
        }


        private void scrollOptions() {
            // SelectedOption too high
            if (SelectedOption.Y < 0) {
                int dy = -(SelectedOption.Y);
                foreach (OdgeButton op in Options)
                    op.Y += (dy / 2) + 1;
            }

            // SelectedOption too low
            else if (SelectedOption.Dimensions.Bottom > bpanel.Height) {
                int dy = SelectedOption.Dimensions.Bottom - bpanel.Height;
                foreach (OdgeButton op in Options)
                    op.Y -= (dy / 2) + 1;
            }
        }
    }
}
