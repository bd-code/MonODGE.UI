using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    /*
    Need:
    TextBox
    Boolean Options (yes/no) or OptionButtonList
    */

    public abstract class Component {
        public enum SnapAnchors {
            TOPLEFT, TOPCENTER, TOPRIGHT,
            LEFT, CENTER, RIGHT,
            BOTTOMLEFT, BOTTOM, BOTTOMRIGHT
        }

        private StyleSheet _style;
        public virtual StyleSheet Style {
            get { return _style; }
            set { _style = value; }
        }

        private Rectangle _dimensions;
        public virtual Rectangle Dimensions {
            get { return _dimensions; }
            set {
                Rectangle old = _dimensions;
                _dimensions = value;

                if (value.Width != old.Width || value.Height != old.Height)
                    OnResize();

                if (value.X != old.X || value.Y != old.Y)
                    OnMove();
            }
        }

        internal CityUIManager _manager;

        /// <summary>
        /// Initialize is called when the Component is added to the UI Manager,
        /// and should not be called manually.
        /// 
        /// Initialize is intended to be overriden when the user needs to perform 
        /// any Component initialization after it is certain to be assigned a manager.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// This is called when the Component's Dimensions.X or Dimensions.Y positions change.
        /// This should be overriden to reposition any text, textures, or other elements 
        /// when the Component's position changes.
        /// </summary>
        public virtual void OnMove() { }

        /// <summary>
        /// This is called when the Component's Dimensions.Width or Dimensions.Height variables
        /// change. This can be overriden if any additional calculations or resizing needs to
        /// occur.
        /// </summary>
        public virtual void OnResize() { }

        /// <summary>
        /// Refresh should be called every time changes are made to the StyleSheet or Dimensions 
        /// that result in a repositioning of text or other sub-elements.
        /// </summary>
        public virtual void Refresh() { }

        public virtual void Update() { }
        public virtual void Draw(SpriteBatch batch) { }


        /// <summary>
        /// Draws the Texture2D saved in Style.Background in color Style.BackgroundColor
        /// to Dimensions.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        protected void DrawCanvas(SpriteBatch batch) {
            if (Style.Background != null)
                batch.Draw(Style.Background, Dimensions, Style.BackgroundColor);
        }


        protected void DrawCorners(SpriteBatch batch) {
            if (Style.Borders != null) {
                int width = Style.BorderTileWidth;
                int height = Style.BorderTileHeight;

                batch.Draw(Style.Borders, new Vector2(Dimensions.X, Dimensions.Y), Style.BorderSourceRects[0], Style.BorderColor);
                batch.Draw(Style.Borders, new Vector2(Dimensions.X + Dimensions.Width - width, Dimensions.Y), Style.BorderSourceRects[2], Style.BorderColor);
                batch.Draw(Style.Borders, new Vector2(Dimensions.X, Dimensions.Y + Dimensions.Height - height), Style.BorderSourceRects[6], Style.BorderColor);
                batch.Draw(Style.Borders, new Vector2(Dimensions.X + Dimensions.Width - width, Dimensions.Y + Dimensions.Height - height), Style.BorderSourceRects[8], Style.BorderColor);
            }
        }


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
                    batch.Draw(Style.Borders, dests[b], Style.BorderSourceRects[b], Style.BorderColor);
            }
        }


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


    public abstract class Control : Component {
        public Control(StyleSheet style) {
            Style = style;
        }
        
        public void Close() { _manager?.CloseControl(this); }
    }

    public abstract class PopUpComponent : Component {
        public PopUpComponent(StyleSheet style) {
            Style = style;
        }
        public void Close() { _manager?.ClearPopUp(this); }
    }
}