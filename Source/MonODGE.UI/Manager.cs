using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonODGE.UI.Components;

namespace MonODGE.UI {
    public class CityUIManager {
        private GraphicsDevice _graphics;

        private Stack<Control> controlStack;
        private Queue<PopUpComponent> popupQ;

        private StyleSheet _style;
        public StyleSheet GlobalStyle {
            get { return _style.Clone(); }
            set { _style = value; }
        }

        internal CityInput Input { get; private set; }
        
        public int ScreenWidth { get { return _graphics.Viewport.Width; } }
        public int ScreenHeight { get { return _graphics.Viewport.Height; } }
        public int ControlCount { get { return controlStack.Count; } }
        public int PopUpCount { get { return popupQ.Count; } }
        
        public CityUIManager(GraphicsDevice graphics, StyleSheet stylesheet) {
            _graphics = graphics;

            controlStack = new Stack<Control>();
            popupQ = new Queue<PopUpComponent>();

            GlobalStyle = stylesheet;
            Input = new CityInput();
        }


        public void Update(KeyboardState keystate) {
            Input.Update(keystate);
        }


        public void UpdateControl() {
            if (controlStack.Count > 0)
                controlStack.Peek().Update();
        }


        public void UpdatePopup() {
            if (popupQ.Count > 0)
                popupQ.Peek().Update();
        }


        public void DrawControl(SpriteBatch batch) {
            if (controlStack.Count > 0)
                controlStack.Peek().Draw(batch);
        }

        public void DrawPopup(SpriteBatch batch) {
            if (popupQ.Count > 0)
                popupQ.Peek().Draw(batch);
        }


        public void Add(Control control) {
            if (control._manager != null && control._manager != this)
                throw new CityComponentUsedException("CityControl added to CityUIManager has already been added to another CityUIManager.");
            control._manager = this;

            if (control.Style == null)
                control.Style = GlobalStyle;

            controlStack.Push(control);
            //control.Initialize();
        }


        public void Add(PopUpComponent popup) {
            if (popup._manager != null && popup._manager != this)
                throw new CityComponentUsedException("CityPopUp added to CityUIManager has already been added to another CityUIManager");
            popup._manager = this;

            if (popup.Style == null)
                popup.Style = GlobalStyle;

            popupQ.Enqueue(popup);
        }


        public void CloseControl(Control control) {
            // Do we need to pass it in?
            controlStack.Pop();
        }

        public void ClearPopUp(PopUpComponent popup) {
            // Do we need to pass it in?
            popupQ.Dequeue();
        }
    }
}
