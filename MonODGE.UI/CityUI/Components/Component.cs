using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CityUI.Components {
    /* 
    Project Names: Gooi, Pooi, BuiGui, 
    DuiGui (Driving under influence of graphical user interfaces),
    BGUI, BsGUI (Bob's shitty GUI)

    Drop the 'G':
    BUI, DUI, PUI, SUI (Shitty UI), 
    BSUI (Bob's Shitty UI)
    ShtiUI, CityUI (as in City Wok)
    */

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

        public StyleSheet Style { get; set; }
        public Rectangle Dimensions { get; set; }

        internal CityUIManager _manager;

        public virtual void Update() { }
        public virtual void Draw(SpriteBatch batch) { }


        protected void DrawCanvas(SpriteBatch batch) {
            if (Style.Background != null)
                batch.Draw(Style.Background, Dimensions, Style.BackgroundColor);
        }


        protected void DrawCorners(SpriteBatch batch) {
            if (Style.Corners != null) {
                int width = Style.CornerSourceRectangles[0].Width;
                int height = Style.CornerSourceRectangles[0].Height;

                batch.Draw(Style.Corners, new Vector2(Dimensions.X, Dimensions.Y), Style.CornerSourceRectangles[0], Style.CornerColor);
                batch.Draw(Style.Corners, new Vector2(Dimensions.X + Dimensions.Width - width, Dimensions.Y), Style.CornerSourceRectangles[1], Style.CornerColor);
                batch.Draw(Style.Corners, new Vector2(Dimensions.X, Dimensions.Y + Dimensions.Height - height), Style.CornerSourceRectangles[2], Style.CornerColor);
                batch.Draw(Style.Corners, new Vector2(Dimensions.X + Dimensions.Width - width, Dimensions.Y + Dimensions.Height - height), Style.CornerSourceRectangles[3], Style.CornerColor);
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

        //internal virtual void Initialize() { }
        public void Close() { _manager?.CloseControl(this); }
    }

    public abstract class PopUpComponent : Component {
        public PopUpComponent(StyleSheet style) {
            Style = style;
        }
        public void Close() { _manager?.ClearPopUp(this); }
    }
}