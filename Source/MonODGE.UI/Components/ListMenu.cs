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

        private ListMenuOptionPanel optionPanel;

        private bool isCancelable;

        public AbstractMenuOption SelectedOption { get { return optionPanel.SelectedOption; } }

        public override StyleSheet Style {
            get { return base.Style; }
            set {
                base.Style = value;
                if (optionPanel != null)
                    optionPanel.Style = value;
            }
        }

        public override Rectangle Dimensions {
            get { return base.Dimensions; }
            set {
                optionPanel.Dimensions = new Rectangle(
                    value.X + Style.Padding,
                    value.Y + (int)textDimensions.Y + Style.Padding,
                    value.Width - Style.Padding * 2,
                    value.Height - (int)textDimensions.Y - Style.Padding * 2
                );
                
                // Resize only if title or panel is too wide.
                int width = MathHelper.Max(
                    Dimensions.Width,
                    MathHelper.Max(
                        optionPanel.Dimensions.Width + Style.Padding * 2,  // <- In case options are wider.
                        (int)textDimensions.X + Style.Padding * 2          // <- In case title is wider.
                    )
                );

                base.Dimensions = new Rectangle(value.X, value.Y, width, value.Height);
            }
        }


        public ListMenu(StyleSheet style, string heading, Rectangle area, bool canCancel = true) : 
            this(style, heading, new List<AbstractMenuOption>(), area, canCancel) { }

        public ListMenu(StyleSheet style, string heading, List<AbstractMenuOption> listOptions, Rectangle area, bool canCancel = true) 
            : base(style) {
            // Set local privates first.
            title = heading;
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);
            isCancelable = canCancel;

            // First create option panel and cascade StyleSheet all the way down.
            optionPanel = new ListMenuOptionPanel(this, listOptions);
            CascadeStyle();

            // Initialize option dimensions, then cascade real Dimension down.
            foreach (AbstractMenuOption option in listOptions)
                option.Dimensions = new Rectangle(-1, -1, 0, 0);
            Dimensions = area;
        }


        public override void Initialize() {
            optionPanel.Initialize();
        }


        public override void OnStyleSet() {
            textDimensions = string.IsNullOrEmpty(title) ? Vector2.Zero : (Style.HeaderFont?.MeasureString(title) ?? Vector2.Zero);
            // FIX: Set new Dimensions.
        }


        public override void OnMove() {
            // Text Positioning
            if (Style.TextAlign == StyleSheet.TextAlignments.LEFT) {
                textPosition = new Vector2(
                    Dimensions.X + Style.Padding,
                    Dimensions.Y + Style.Padding
                );
            }
            else if (Style.TextAlign == StyleSheet.TextAlignments.CENTER) {
                textPosition = new Vector2(
                    (Dimensions.Width - textDimensions.X) / 2 + Dimensions.X,
                    Dimensions.Y + Style.Padding
                );
            }
            else { // Right
                textPosition = new Vector2(
                    Dimensions.Width - textDimensions.X - Style.Padding + Dimensions.X,
                    Dimensions.Y + Style.Padding
                );
            }
        }


        public override void Update() {
            if (_manager.Input.isKeyPress(Style.CancelKey) && isCancelable) {
                Close();
            }

            optionPanel.Update();
        }


        public override void Draw(SpriteBatch batch) {
            DrawCanvas(batch);
            DrawBorders(batch);
            if (title != null)
                batch.DrawString(Style.HeaderFont, title, textPosition, Style.HeaderColor);
            optionPanel.Draw(batch);
        }

        /// <summary>
        /// Cascades ListMenu's StyleSheet to ListMenuOptionPanel and all (or some, see forced 
        /// param) AbstractListMenuOptions in ListMenuOptionPanel.
        /// 
        /// It is recommended to call Refresh after CascadeStyle to reposition all elements.
        /// </summary>
        /// <param name="forced">
        /// If true, sets option's StyleSheet even if the option already has one.
        /// If false (default), only sets option's StyleSheet if it is null.
        /// </param>
        public void CascadeStyle(bool forced = false) {
            optionPanel.Style = Style;
            optionPanel.CascadeStyle(forced);
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Internal: A container for AbstractListMenuOption objects for use in a ListMenu.
    /// </summary>
    internal class ListMenuOptionPanel : OdgeComponent {
        private ListMenu parent;
        private RenderTarget2D panel;
        private SpriteBatch panelBatch;

        internal List<AbstractMenuOption> Options { get; private set; }
        private int selectedIndex;

        public AbstractMenuOption SelectedOption { get { return Options[selectedIndex]; } }

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
                int width = value.Width;

                // Set initial option widths.
                foreach (AbstractMenuOption option in Options) {
                    option.Dimensions = new Rectangle(
                        option.Dimensions.X,
                        option.Dimensions.Y,
                        width,
                        option.Dimensions.Height
                        );

                    if (option.Dimensions.Width > width)
                        width = option.Dimensions.Width;
                }

                // Resize options again to full width.
                foreach (AbstractMenuOption option in Options) {
                    if (option.Dimensions.Width < width)
                        option.Dimensions = new Rectangle(
                            option.Dimensions.X,
                            option.Dimensions.Y,
                            width,
                            option.Dimensions.Height
                            );
                }

                base.Dimensions = new Rectangle(value.X, value.Y, width, value.Height);

                if (parent._manager != null)
                    panel = parent._manager.CreateRenderTarget(Dimensions.Width, Dimensions.Height);
            }
        }


        internal ListMenuOptionPanel(ListMenu parentMenu, List<AbstractMenuOption> listOptions) {
            parent = parentMenu;
            Options = listOptions;
        }


        public override void Initialize() {
            panel = parent._manager.CreateRenderTarget(Dimensions.Width, Dimensions.Height);
            panelBatch = new SpriteBatch(parent._manager.GraphicsDevice);
            foreach (AbstractMenuOption option in Options)
                option.Initialize();
        }


        public override void OnMove() {
            // Set the option positions.
            int ypos = 0;
            foreach (AbstractMenuOption option in Options) {
                option.Dimensions = new Rectangle(0, ypos, option.Dimensions.Width, option.Dimensions.Height);
                ypos += option.Dimensions.Height;
            }
        }


        public override void Update() {
            // Handle input.
            if (parent._manager.Input.isKeyPress(Style.SubmitKey)) {
                Options[selectedIndex].OnSubmit();
            }

            // Move Down.
            else if (parent._manager.Input.isKeyPress(Keys.Down) || parent._manager.Input.isKeyPress(Keys.S)) {
                Options[selectedIndex].OnUnselected();
                selectedIndex++;
                if (selectedIndex >= Options.Count)
                    selectedIndex = 0;
                Options[selectedIndex].OnSelected();
            }

            // Move Up!
            else if (parent._manager.Input.isKeyPress(Keys.Up) || parent._manager.Input.isKeyPress(Keys.W)) {
                Options[selectedIndex].OnUnselected();
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = Options.Count - 1;
                Options[selectedIndex].OnSelected();
            }


            // Get top option position.
            //int yPos = Dimensions.Y;

            // Reposition items if they are off screen.            
            /* FIX
            if (Options[selectedIndex].Dimensions.Y < yPos) {
                velocity = -(Options[selectedIndex].Dimensions.Y / 2) + 1;
                scrollList();
            }
            else if (Options[selectedIndex].Dimensions.Y + Options[selectedIndex].Dimensions.Height > _manager.ScreenHeight) {
                velocity = -(Options[selectedIndex].Dimensions.Y + Options[selectedIndex].Dimensions.Height / 2) - 1;
                scrollList();
            }
            */

            // Update option rects
            /*foreach (AbstractListMenuOption option in Options) {
                Rectangle place = option.Dimensions;
                place.Y = yPos;
                option.Dimensions = place;
                yPos += option.Dimensions.Height;

                // option.Update(selected);
            }*/
        }


        private void DrawToPanel() {
            parent._manager.GraphicsDevice.SetRenderTarget(panel);
            parent._manager.GraphicsDevice.Clear(Color.TransparentBlack);
            panelBatch.Begin();

            for (int p = 0; p < Options.Count; p++) {
                if (p == selectedIndex)
                    Options[p].Draw(panelBatch, true);
                else
                    Options[p].Draw(panelBatch, false);
            }

            panelBatch.End();
            parent._manager.GraphicsDevice.SetRenderTarget(null);
        }


        public override void Draw(SpriteBatch batch) {
            DrawToPanel();
            batch.Draw(panel, Dimensions, Color.White);
        }


        /*private void scrollList() {            
            foreach (AbstractListMenuOption option in Options) {
                Rectangle dim = option.Dimensions;
                dim.Y += velocity;
                option.Dimensions = dim;
            }          
        }*/

        /// <summary>
        /// Cascades ListMenuOptionPanel's StyleSheet to all (or some, see forced param) 
        /// AbstractListMenuOptions.
        /// 
        /// It is recommended to call Refresh after CascadeStyle to reposition all elements.
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
