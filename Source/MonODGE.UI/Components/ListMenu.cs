using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Components {
    public class ListMenu : Control {
        /*
        Needs Add(option) and Remove(option) for dynamic lists (think inventory).
        Re-design with title and scrolling ListOptionPanel.
        */
        public override StyleSheet Style {
            get { return base.Style; }

            set {
                base.Style = value;

                // Cascade StyleSheet down.
                if (Options != null) {
                    foreach (AbstractListMenuOption option in Options) {
                        option.Style = Style;
                        option.Measure();
                    }
                    Refresh();
                }
            }
        }
        public List<AbstractListMenuOption> Options { get; private set; }

        private int selectedIndex;
        private string title;  // < This isn't drawn anywhere.
        private bool isCancelable;
        private int velocity;

        public AbstractListMenuOption SelectedOption { get { return Options[selectedIndex]; } }

        public ListMenu(StyleSheet style, string heading, Rectangle area, bool canCancel = true) : 
            this(style, heading, new List<AbstractListMenuOption>(), area, canCancel) { }

        public ListMenu(StyleSheet style, string heading, List<AbstractListMenuOption> listOptions, Rectangle area, bool canCancel = true) 
            : base(style) {
            title = heading;
            Dimensions = area;
            Options = listOptions;

            // Cascade StyleSheet down.
            foreach (AbstractListMenuOption option in Options) {
                option.Style = Style;
                option.Measure();
            }

            isCancelable = canCancel;
            Refresh();
        }

        public ListMenu(CityUIManager manager, string heading, Rectangle area, bool canCancel = true) :
            this(manager.GlobalStyle, heading, new List<AbstractListMenuOption>(), area, canCancel) {
            manager.Add(this);
        }

        public ListMenu(CityUIManager manager, string heading, List<AbstractListMenuOption> listOptions, Rectangle area, bool canCancel = true) :
            this(manager.GlobalStyle, heading, listOptions, area, canCancel) {
            manager.Add(this);
        }


        public override void Refresh() {
            /*
            Refresh should really cascade Style down, then call Refresh to recalc positions.
            */

            // Calculates and sets the initial positions 
            // and dimensions of all list options.
            int width = Dimensions.Width;

            // First get max width of options.
            foreach (AbstractListMenuOption option in Options) {   
                if (option.Dimensions.Width > width)
                    width = option.Dimensions.Width;
            }

            // Now set the option dimensions.
            int ypos = Dimensions.Y;
            foreach (AbstractListMenuOption option in Options) {
                int height = option.Dimensions.Height;
                option.Dimensions = new Rectangle(Dimensions.X, ypos, width, height);
                ypos += option.Dimensions.Height;
            }
        }


        public override void SnapTo(SnapAnchors anchor, int screenwidth, int screenheight) {
            base.SnapTo(anchor, screenwidth, screenheight);
            foreach (AbstractListMenuOption option in Options)
                option.SnapTo(anchor, screenwidth, screenheight);
        }


        public override void Update() {
            // Handle input.
            if (_manager.Input.isKeyPress(Style.SubmitKey)) {
                Options[selectedIndex].OnConfirm();
            }

            // Close ListMenu should only work if isCancelable.
            else if (_manager.Input.isKeyPress(Style.CancelKey) && isCancelable) {
                Close();
            }

            // Move Down.
            else if (_manager.Input.isKeyPress(Keys.Down) || _manager.Input.isKeyPress(Keys.S)) {
                selectedIndex++;
                if (selectedIndex >= Options.Count)
                    selectedIndex = 0;
            }

            // Move Up!
            else if (_manager.Input.isKeyPress(Keys.Up) || _manager.Input.isKeyPress(Keys.W)) {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = Options.Count - 1;
            }
            
            // Get top option position.
            int yPos = Dimensions.Y;
            if (!string.IsNullOrEmpty(title))
                yPos += (int)Style.HeaderFont.MeasureString(title).Y * 2;
            
            // Reposition items if they are off screen.            
            if (Options[selectedIndex].Dimensions.Y < yPos) {
                velocity = -(Options[selectedIndex].Dimensions.Y / 2) + 1;
                scrollList();
            }
            else if (Options[selectedIndex].Dimensions.Y + Options[selectedIndex].Dimensions.Height > _manager.ScreenHeight) {
                velocity = -(Options[selectedIndex].Dimensions.Y + Options[selectedIndex].Dimensions.Height / 2) - 1;
                scrollList();
            }
                        
            // Update option rects
            foreach (AbstractListMenuOption option in Options) {
                Rectangle place = option.Dimensions;
                place.Y = yPos;
                option.Dimensions = place;
                yPos += option.Dimensions.Height;
            }
        }


        public override void Draw(SpriteBatch batch) {
            for (int p = 0; p < Options.Count; p++) {
                if (p == selectedIndex)
                    Options[p].Draw(batch, true);
                else
                    Options[p].Draw(batch, false);
            }
        }

        /// <summary>
        /// This is deprecated, but we still need options to add and remove options.
        /// (Think inventory.)
        /// </summary>
        /*public void AddOptions(List<AbstractListMenuOption> options) {
            // Expensive, as it calls Initialize() to recalculate option
            // positions every call. It is preferred to pass in options via
            // the constructor.
            Options = options;
            Refresh();
        }*/


        private void scrollList() {            
            foreach (AbstractListMenuOption option in Options) {
                Rectangle dim = option.Dimensions;
                dim.Y += velocity;
                option.Dimensions = dim;
            }          
        }
    }
}
