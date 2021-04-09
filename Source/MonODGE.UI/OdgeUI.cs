
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Components;

namespace MonODGE.UI {
    /// <summary>
    /// <para>The core engine of the MonODGE.UI library.</para>
    /// 
    /// <para>It is comparable to a top-level Window, Screen, or Application 
    /// object found in other UI libraries.</para>
    /// </summary>
    public class OdgeUI {
        public GraphicsDevice GraphicsDevice { get; }

        private Texture2D mask;

        private Stack<OdgeControl> controlStack;
        private Queue<OdgePopUp> popupQ;
        public StyleSheet GlobalStyle { get; set; }

        public int ScreenWidth { get { return GraphicsDevice.Viewport.Width; } }
        public int ScreenHeight { get { return GraphicsDevice.Viewport.Height; } }
        public int ControlCount { get { return controlStack.Count; } }
        public int PopUpCount { get { return popupQ.Count; } }

        //// Config settings ////

        /// <summary>
        /// Toggles whether to draw all open OdgeControls or just the active one.
        /// </summary>
        public bool DrawAllControls { get; set; }

        /// <summary>
        /// Toggles whether to draw a mask over inactive OdgeControls when DrawAllControls == true.
        /// </summary>
        public bool DrawInactiveMask { get; set; }

        /// <summary>
        /// Toggles whether to update all open OdgePopUps together or just the current one.
        /// </summary>
        public bool RunAllPopUps { get; set; }
        
        public OdgeUI(GraphicsDevice graphics, StyleSheet stylesheet) {
            GraphicsDevice = graphics;
            mask = new Texture2D(graphics, 1, 1);
            mask.SetData(new Color[] { Color.White });

            controlStack = new Stack<OdgeControl>();
            popupQ = new Queue<OdgePopUp>();

            GlobalStyle = stylesheet;

            // Config options.
            DrawAllControls = false;
            DrawInactiveMask = true;
            RunAllPopUps = false;
        }


        public void UpdateControl() {
            if (controlStack.Count > 0) {

                /* Style Changes:
                As components can (and should) share StyleSheets, if a shared StyleSheet changes 
                we need to run OnStyleChanged() on each individual component that has it, THEN loop
                through again to run StyleSheet.AcceptChanges(). Otherwise we will only notify one
                component before we unflag the change. 
                */

                // Check for style changes.
                foreach (OdgeControl odge in controlStack) {
                    if (odge.Style.IsChanged) {
                        odge.OnStyleChanged();
                    }
                }

                // Reset Style.IsChanged flag separately, to avoid skips.
                foreach (OdgeControl odge in controlStack) {
                    if (odge.Style.IsChanged)
                        odge.Style.AcceptChanges();
                }

                controlStack.Peek().Update();
            }
        }


        public void UpdatePopup() {
            if (popupQ.Count > 0) {
                // Check for style changes.
                foreach (OdgePopUp odge in popupQ) {
                    if (odge.Style.IsChanged) {
                        odge.OnStyleChanged();
                        odge.Style.AcceptChanges();
                    }
                }

                // Reset Style.IsChanged flag separately, to avoid skips.
                foreach (OdgePopUp odge in popupQ) {
                    if (odge.Style.IsChanged)
                        odge.Style.AcceptChanges();
                }



                if (RunAllPopUps) {
                    int c = popupQ.Count;
                    for (int p = 0; p < c; p++) {
                        OdgePopUp pop = popupQ.Dequeue();
                        if (pop.Timeout > 0) {
                            pop.Update();
                            popupQ.Enqueue(pop);
                        }
                    }
                }
                else {
                    if (popupQ.Peek().Timeout > 0)
                        popupQ.Peek().Update();
                    else
                        popupQ.Dequeue();
                }
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


        public void Open(OdgeControl control) {
            if (control._manager != null && control._manager != this)
                throw new OdgeComponentUsedException("OdgeControl has already been added to another OdgeUI manager.");
            control._manager = this;

            if (control.Style == null)
                control.Style = GlobalStyle;

            controlStack.Push(control);
            control.OnOpened();
        }


        public void Open(OdgePopUp popup) {
            if (popup._manager != null && popup._manager != this)
                throw new OdgeComponentUsedException("OdgePopUp has already been added to another OdgeUI manager.");
            popup._manager = this;

            if (popup.Style == null)
                popup.Style = GlobalStyle;

            popupQ.Enqueue(popup);
            popup.OnOpened();
        }


        public void Close(OdgeControl control) {
            Stack<OdgeControl> temp = new Stack<OdgeControl>();

            while (controlStack.Count > 0) {
                OdgeControl con = controlStack.Pop();
                if (con == control)
                    break;
                else
                    temp.Push(con);
            }

            while (temp.Count > 0)
                controlStack.Push(temp.Pop());
        }


        public void Close(OdgePopUp popup) {
            int c = popupQ.Count;
            for (int p = 0; p < c; p++) {
                OdgePopUp pop = popupQ.Dequeue();
                if (!(pop == popup))
                    popupQ.Enqueue(pop);
            }
        }


        /// <summary>
        /// Find an open OdgeComponent by its Name property. Searches both OdgeControls and OdgePopUps.
        /// Note the return value will be an OdgeComponent, and must be cast to the appropriate subtype.
        /// </summary>
        /// <param name="name">string name of OdgeComponent.</param>
        /// <returns></returns>
        public OdgeComponent GetComponentByName(string name) {
            OdgeComponent odge = GetControlByName(name);
            if (odge == null)
                odge = GetPopupByName(name);
            return odge;
        }
        public OdgeControl GetControlByName(string name) {
            foreach (OdgeControl odge in controlStack)
                if (odge.Name == name)
                    return odge;
            return null;
        }
        public OdgePopUp GetPopupByName(string name) {
            foreach (OdgePopUp pop in popupQ)
                if (pop.Name == name)
                    return pop;
            return null;
        }


        public void SetAllComponentStyle() {
            foreach (OdgeControl control in controlStack)
                control.Style = GlobalStyle;
            foreach (OdgePopUp pop in popupQ)
                pop.Style = GlobalStyle;
        }
        public void SetAllComponentStyle(StyleSheet style) {
            foreach (OdgeControl control in controlStack)
                control.Style = style;
            foreach (OdgePopUp pop in popupQ)
                pop.Style = style;
        }
    }
}
