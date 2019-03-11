using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI {
    internal class OdgeInput {
        private KeyboardState prevState;
        private KeyboardState state;

        public OdgeInput() {
            prevState = Keyboard.GetState();
            state = Keyboard.GetState();
        }

        public void Update(KeyboardState keystate) {
            prevState = state;
            state = keystate;
        }

        public bool isKeyDown(Keys kee) {
            return state.IsKeyDown(kee);
        }

        public bool isKeyHold(Keys kee) {
            return (state.IsKeyDown(kee) && prevState.IsKeyDown(kee));
        }

        public bool isKeyPress(Keys kee) {
            return (state.IsKeyDown(kee) && !prevState.IsKeyDown(kee));
        }

        public bool isKeyUp(Keys kee) {
            return (!state.IsKeyDown(kee) && prevState.IsKeyDown(kee));
        }

        public bool anyKey() {
            return (state.GetPressedKeys().Length > 0 && prevState.GetPressedKeys().Length == 0);
        }

        public void flush() {
            state = prevState;
        }
    }
}
