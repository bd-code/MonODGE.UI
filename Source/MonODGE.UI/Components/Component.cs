using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    public abstract class OdgeComponent {
        public enum SnapAnchors {
            TOPLEFT, TOPCENTER, TOPRIGHT,
            LEFT, CENTER, RIGHT,
            BOTTOMLEFT, BOTTOM, BOTTOMRIGHT
        }

        internal OdgeUI _manager;

        public string Name { get; set; }


        private StyleSheet _style;
        public virtual StyleSheet Style {
            get { return _style; }
            set {
                _style = value;
                _style.RegisterChanges();
            }
        }


        private Rectangle _dimensions;
        public virtual Rectangle Dimensions {
            get { return _dimensions; }
            set {
                Rectangle old = _dimensions;
                value.Width = MathHelper.Max(value.Width, MinWidth);
                value.Height = MathHelper.Max(value.Height, MinHeight);
                _dimensions = value;

                if (value.Width != old.Width || value.Height != old.Height)
                    OnResize();

                if (value.X != old.X || value.Y != old.Y)
                    OnMove();
            }
        }

        public virtual int X {
            get { return _dimensions.X; }
            set { Dimensions = new Rectangle(value, Y, Width, Height); }
        }
        public virtual int Y {
            get { return _dimensions.Y; }
            set { Dimensions = new Rectangle(X, value, Width, Height); }
        }
        public virtual int Width {
            get { return _dimensions.Width; }
            set { Dimensions = new Rectangle(X, Y, value, Height); }
        }
        public virtual int Height {
            get { return _dimensions.Height; }
            set { Dimensions = new Rectangle(X, Y, Width, value); }
        }

        protected virtual int MinWidth { get { return 0; } }
        protected virtual int MinHeight { get { return 0; } }


        /// <summary>
        /// This is called when the OdgeComponent is added to the UI Manager.
        /// </summary>
        public virtual void OnOpened() { Opened?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Opened;

        /// <summary>
        /// This is called when a property changes in the OdgeComponent's StyleSheet,
        /// or when setting OdgeComponent.Style to a new StyleSheet.
        /// </summary>
        public virtual void OnStyleChanged() { StyleChanged?.Invoke(this, EventArgs.Empty); }
        public event EventHandler StyleChanged;

        /// <summary>
        /// This is called when the OdgeComponent's Dimensions.X or Dimensions.Y positions change.
        /// This should be overriden to reposition any text, textures, or other elements 
        /// when the Component's position changes.
        /// </summary>
        public virtual void OnMove() { Move?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Move;

        /// <summary>
        /// This is called when the OdgeComponent's Dimensions.Width or Dimensions.Height variables
        /// change. This can be overriden if any additional calculations or resizing needs to
        /// occur.
        /// 
        /// OnResize should not be used to re-resize Dimensions, as this will recursively call 
        /// OnResize(). If we need to check that the new width and height are large enough to 
        /// contain sub-elements, override the Dimensions property itself to check the values.
        /// </summary>
        public virtual void OnResize() { Resize?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Resize;

        /// <summary>
        /// This is called when the OdgeComponent is closed.
        /// </summary>
        public virtual void OnClosed() { Closed?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Closed;

        /*
        /// <summary>
        /// Refresh should be called every time changes are made to the StyleSheet or Dimensions 
        /// that result in a repositioning of text or other sub-elements.
        /// </summary>
        public virtual void Refresh() { }
        */

        public virtual void Update() { }
        public virtual void Draw(SpriteBatch batch) { }
        public virtual void Draw(SpriteBatch batch, Rectangle parentRect) { }


        /// <summary>
        /// Draws the Texture2D saved in Style.Background in color Style.BackgroundColor
        /// to the position and area saved in Dimensions.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        protected void DrawBG(SpriteBatch batch) {
            if (Style.Background != null)
                batch.Draw(Style.Background, Dimensions, Style.BackgroundColor);
        }


        /// <summary>
        /// Draws the Texture2D saved in Style.Background in color Style.BackgroundColor
        /// to the position and area saved in Rectangle 'where'.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="where">Rectangle area to draw.</param>
        protected void DrawBG(SpriteBatch batch, Rectangle where) {
            if (Style.Background != null)
                batch.Draw(Style.Background, where, Style.BackgroundColor);
        }


        /// <summary>
        /// Draws only the corner tiles of Style.Borders to the four corners saved in Dimensions.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        protected void DrawCorners(SpriteBatch batch) {
            if (Style.Borders != null) {
                batch.Draw(Style.Borders, 
                    new Vector2(Dimensions.X, Dimensions.Y), 
                    Style.BorderSourceRects[0], Style.BorderColor);
                batch.Draw(Style.Borders, 
                    new Vector2(Dimensions.X + Dimensions.Width - Style.BorderTileWidth, Dimensions.Y), 
                    Style.BorderSourceRects[2], Style.BorderColor);
                batch.Draw(Style.Borders, 
                    new Vector2(Dimensions.X, Dimensions.Y + Dimensions.Height - Style.BorderTileHeight), 
                    Style.BorderSourceRects[6], Style.BorderColor);
                batch.Draw(Style.Borders, 
                    new Vector2(Dimensions.X + Dimensions.Width - Style.BorderTileWidth, Dimensions.Y + Dimensions.Height - Style.BorderTileHeight), 
                    Style.BorderSourceRects[8], Style.BorderColor);
            }
        }


        /// <summary>
        /// Draws only the corner tiles of Style.Borders to the four corners saved in 
        /// Rectangle 'where'.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="where">Rectangle area to draw.</param>
        protected void DrawCorners(SpriteBatch batch, Rectangle where) {
            if (Style.Borders != null) {
                batch.Draw(Style.Borders,
                    new Vector2(where.X, where.Y),
                    Style.BorderSourceRects[0], Style.BorderColor);
                batch.Draw(Style.Borders,
                    new Vector2(where.X + where.Width - Style.BorderTileWidth, where.Y),
                    Style.BorderSourceRects[2], Style.BorderColor);
                batch.Draw(Style.Borders,
                    new Vector2(where.X, where.Y + where.Height - Style.BorderTileHeight),
                    Style.BorderSourceRects[6], Style.BorderColor);
                batch.Draw(Style.Borders,
                    new Vector2(where.X + where.Width - Style.BorderTileWidth, where.Y + where.Height - Style.BorderTileHeight),
                    Style.BorderSourceRects[8], Style.BorderColor);
            }
        }


        /// <summary>
        /// Draws the Texture2D saved in Style.Borders around the OdgeComponent.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        protected void DrawBorders(SpriteBatch batch) {
            if (Style.Borders != null) {
                Rectangle[] dests = new Rectangle[] {
                    new Rectangle(Dimensions.X, Dimensions.Y, Style.BorderTileWidth, Style.BorderTileHeight),
                    new Rectangle(Dimensions.X + Style.BorderTileWidth, Dimensions.Y, Dimensions.Width - Style.BorderTileWidth*2, Style.BorderTileHeight),
                    new Rectangle(Dimensions.X + Dimensions.Width - Style.BorderTileWidth, Dimensions.Y, Style.BorderTileWidth, Style.BorderTileHeight),

                    new Rectangle(Dimensions.X, Dimensions.Y + Style.BorderTileHeight, Style.BorderTileWidth, Dimensions.Height - Style.BorderTileHeight*2),
                    new Rectangle(), // Center is empty.
                    new Rectangle(Dimensions.X + Dimensions.Width - Style.BorderTileWidth, Dimensions.Y + Style.BorderTileHeight, Style.BorderTileWidth, Dimensions.Height - Style.BorderTileHeight*2),

                    new Rectangle(Dimensions.X, Dimensions.Y + Dimensions.Height - Style.BorderTileHeight, Style.BorderTileWidth, Style.BorderTileHeight),
                    new Rectangle(Dimensions.X + Style.BorderTileWidth, Dimensions.Y + Dimensions.Height - Style.BorderTileHeight, Dimensions.Width - Style.BorderTileWidth*2, Style.BorderTileHeight),
                    new Rectangle(Dimensions.X + Dimensions.Width - Style.BorderTileWidth, Dimensions.Y + Dimensions.Height - Style.BorderTileHeight, Style.BorderTileWidth, Style.BorderTileHeight),
                };

                for (int b = 0; b < 9; b++)
                    if (b != 4)
                        batch.Draw(Style.Borders, dests[b], Style.BorderSourceRects[b], Style.BorderColor);
            }
        }


        /// <summary>
        /// Draws the Texture2D saved in Style.Borders around Rectangle 'where'.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="where">Rectangle area to draw.</param>
        protected void DrawBorders(SpriteBatch batch, Rectangle where) {
            if (Style.Borders != null) {
                Rectangle[] dests = new Rectangle[] {
                    new Rectangle(where.X, where.Y, Style.BorderTileWidth, Style.BorderTileHeight),
                    new Rectangle(where.X + Style.BorderTileWidth, where.Y, where.Width - Style.BorderTileWidth*2, Style.BorderTileHeight),
                    new Rectangle(where.X + where.Width - Style.BorderTileWidth, where.Y, Style.BorderTileWidth, Style.BorderTileHeight),

                    new Rectangle(where.X, where.Y + Style.BorderTileHeight, Style.BorderTileWidth, where.Height - Style.BorderTileHeight*2),
                    new Rectangle(), // Center is empty.
                    new Rectangle(where.X + where.Width - Style.BorderTileWidth, where.Y + Style.BorderTileHeight, Style.BorderTileWidth, where.Height - Style.BorderTileHeight*2),

                    new Rectangle(where.X, where.Y + where.Height - Style.BorderTileHeight, Style.BorderTileWidth, Style.BorderTileHeight),
                    new Rectangle(where.X + Style.BorderTileWidth, where.Y + where.Height - Style.BorderTileHeight, where.Width - Style.BorderTileWidth*2, Style.BorderTileHeight),
                    new Rectangle(where.X + where.Width - Style.BorderTileWidth, where.Y + where.Height - Style.BorderTileHeight, Style.BorderTileWidth, Style.BorderTileHeight),
                };

                for (int b = 0; b < 9; b++)
                    if (b != 4)
                        batch.Draw(Style.Borders, dests[b], Style.BorderSourceRects[b], Style.BorderColor);
            }
        }


        /// <summary>
        /// Snaps the OdgeComponent to a screen position specified in SnapAnchors.
        /// </summary>
        /// <param name="anchor">A SnapAnchors enum value.</param>
        /// <param name="screenwidth">Screen width.</param>
        /// <param name="screenheight">Screen height.</param>
        public virtual void SnapTo(SnapAnchors anchor, int screenwidth, int screenheight) {
            if (anchor == SnapAnchors.TOPLEFT) {
                Dimensions = new Rectangle(0, 0, Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.TOPCENTER) {
                Dimensions = new Rectangle(
                    (screenwidth - Dimensions.Width) / 2,
                    0,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.TOPRIGHT) {
                Dimensions = new Rectangle(
                    screenwidth - Dimensions.Width,
                    0,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.LEFT) {
                Dimensions = new Rectangle(
                    0,
                    (screenheight - Dimensions.Height) / 2,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.CENTER) {
                Dimensions = new Rectangle(
                    (screenwidth - Dimensions.Width) / 2,
                    (screenheight - Dimensions.Height) / 2,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.RIGHT) {
                Dimensions = new Rectangle(
                    screenwidth - Dimensions.Width,
                    (screenheight - Dimensions.Height) / 2,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.BOTTOMLEFT) {
                Dimensions = new Rectangle(
                    0,
                    screenheight - Dimensions.Height,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.BOTTOM) {
                Dimensions = new Rectangle(
                    (screenwidth - Dimensions.Width) / 2,
                    screenheight - Dimensions.Height,
                    Dimensions.Width, Dimensions.Height);
            }

            else if (anchor == SnapAnchors.BOTTOMRIGHT) {
                Dimensions = new Rectangle(
                    screenwidth - Dimensions.Width,
                    screenheight - Dimensions.Height,
                    Dimensions.Width, Dimensions.Height);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public abstract class OdgeControl : OdgeComponent {
        public OdgeControl(StyleSheet style) {
            Style = style;
        }

        /// <summary>
        /// This is called when the user presses the Key assigned in Style.SubmitKey,
        /// or the GamePad Button assigned in Style.SubmitButton.
        /// </summary>
        public virtual void OnSubmit() { Submit?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Submit;

        /// <summary>
        /// This is called when the user presses the key assigned in Style.CancelKey,
        /// or the GamePad Button assigned in Style.CancelButton.
        /// </summary>
        public virtual void OnCancel() {
            Cancel?.Invoke(this, EventArgs.Empty);
            if (Style.CloseOnCancel)
                Close();
        }
        public event EventHandler Cancel;

        public void Close() {
            if (_manager != null) {
                _manager.Close(this);
                OnClosed();
            }
        }

        protected bool CheckSubmit {
            get { return OdgeUIInput.KB.isKeyTap(Style.SubmitKey) || OdgeUIInput.GP.isButtonTap(Style.SubmitButton); }
        }

        protected bool CheckCancel {
            get { return OdgeUIInput.KB.isKeyTap(Style.CancelKey) || OdgeUIInput.GP.isButtonTap(Style.CancelButton); }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public abstract class OdgePopUp : OdgeComponent {
        public int Timeout { get; set; }

        public OdgePopUp(StyleSheet style) {
            Style = style;
        }

        public void Close() {
            if (_manager != null) {
                _manager.Close(this);
                OnClosed();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public abstract class OdgeButton : OdgeComponent {
        public bool IsSelected { get; private set; }

        public OdgeButton(StyleSheet style, EventHandler action) {
            Style = style;
            Submit += action;
        }

        /// <summary>
        /// This is called when the user presses the key assigned in Style.SubmitKey.
        /// </summary>
        public virtual void OnSubmit() { Submit?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Submit;

        /// <summary>
        /// This is called when the user presses the key assigned in Style.CancelKey.
        /// </summary>
        /// REMOVED
        //public virtual void OnCancel() { Cancel?.Invoke(this, EventArgs.Empty); }
        //public event EventHandler Cancel;

        /// <summary>
        /// This is called when an option is highlighted in the ListMenu.
        /// </summary>
        public virtual void OnSelected() {
            IsSelected = true;
            Select?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Select;

        /// <summary>
        /// This is called when an option is unhighlighted (another option is selected) in the
        /// ListMenu.
        /// </summary>
        public virtual void OnUnselected() {
            IsSelected = false;
            Unselect?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Unselect;
    }

}