using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonODGE.UI.Components;

namespace MonODGE.UI {
    public class OdgeUI {
        private GraphicsDevice _graphics;
        public GraphicsDevice GraphicsDevice { get { return _graphics; } }

        Texture2D mask;

        private Stack<OdgeControl> controlStack;
        private Queue<OdgePopUp> popupQ;

        private StyleSheet _style;
        public StyleSheet GlobalStyle {
            get { return _style; }
            set { _style = value; }
        }

        internal OdgeInput Input { get; private set; }
        
        public int ScreenWidth { get { return _graphics.Viewport.Width; } }
        public int ScreenHeight { get { return _graphics.Viewport.Height; } }
        public int ControlCount { get { return controlStack.Count; } }
        public int PopUpCount { get { return popupQ.Count; } }

        // Config settings
        public bool DrawAllControls { get; set; }
        public bool DrawInactiveMask { get; set; }
        public bool RunAllPopUps { get; set; }
        
        public OdgeUI(GraphicsDevice graphics, StyleSheet stylesheet) {
            _graphics = graphics;
            mask = new Texture2D(graphics, 1, 1);
            mask.SetData(new Color[] { Color.White });

            controlStack = new Stack<OdgeControl>();
            popupQ = new Queue<OdgePopUp>();

            GlobalStyle = stylesheet;
            Input = new OdgeInput();

            DrawInactiveMask = true;
        }


        public void Update(KeyboardState keystate) {
            Input.Update(keystate);
        }


        public void UpdateControl() {
            if (controlStack.Count > 0)
                controlStack.Peek().Update();
        }


        public void UpdatePopup() {
            if (popupQ.Count > 0) {
                if (RunAllPopUps) {
                    OdgePopUp[] pops = popupQ.ToArray();
                    foreach (OdgePopUp pop in pops)
                        pop.Update();
                }
                else
                    popupQ.Peek().Update();
            }
        }


        public void DrawControl(SpriteBatch batch) {
            if (controlStack.Count > 0) {
                if (DrawAllControls) {
                    foreach (OdgeControl odge in controlStack.Reverse()) {
                        odge.Draw(batch);
                        if (DrawInactiveMask && odge != controlStack.Peek())
                            batch.Draw(mask, odge.Dimensions, new Color(0, 0, 0, 128));
                    }
                }
                else
                    controlStack.Peek().Draw(batch);
            }
        }

        public void DrawPopup(SpriteBatch batch) {
            if (popupQ.Count > 0) {
                if (RunAllPopUps) {
                    foreach (OdgePopUp pop in popupQ)
                        pop.Draw(batch);
                }
                else
                    popupQ.Peek().Draw(batch);
            }
        }


        public void Add(OdgeControl control) {
            if (control._manager != null && control._manager != this)
                throw new OdgeComponentUsedException("CityControl added to CityUIManager has already been added to another CityUIManager.");
            control._manager = this;

            if (control.Style == null)
                control.Style = GlobalStyle;

            controlStack.Push(control);
            control.Initialize();
        }


        public void Add(OdgePopUp popup) {
            if (popup._manager != null && popup._manager != this)
                throw new OdgeComponentUsedException("CityPopUp added to CityUIManager has already been added to another CityUIManager");
            popup._manager = this;

            if (popup.Style == null)
                popup.Style = GlobalStyle;

            popupQ.Enqueue(popup);
            popup.Initialize();
        }


        public void CloseControl(OdgeControl control) {
            // Do we need to pass it in?
            controlStack.Pop();
        }

        public void ClearPopUp(OdgePopUp popup) {
            // Do we need to pass it in?
            popupQ.Dequeue();
        }

        public RenderTarget2D CreateRenderTarget(int width, int height) {
            return new RenderTarget2D(_graphics, width, height);
        }
    }
}
