using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Utilities {
    /// <summary>
    /// Provides support and useful methods for Keyboard input.
    /// </summary>
    public class KeyboardHandler {
        private KeyboardState _oldstate;
        private KeyboardState _state;

        public KeyboardHandler() {
            _oldstate = Keyboard.GetState();
            _state = Keyboard.GetState();
        }

        public void Update() {
            _oldstate = _state;
            _state = Keyboard.GetState();
        }

        /// <summary>
        /// This is provided in case user wants to use an external input manager. 
        /// Pass in the KeyboardState of the external input manager to effectively 
        /// "sync" OdgeInput's KeyboardState with the external.
        /// </summary>
        /// <param name="keystate">KeyboardState of the external input library.</param>
        public void Update(KeyboardState keystate) {
            _oldstate = _state;
            _state = keystate;
        }

        public bool isKeyTap(Keys kee) {
            return (_state.IsKeyDown(kee) && !_oldstate.IsKeyDown(kee));
        }
    }


    /// <summary>
    /// Provides support and useful methods for GamePad (controller) input.
    /// </summary>
    public class GamePadHandler {
        private GamePadState _oldstate;
        private GamePadState _state;
        
        public GamePadHandler() {
           _oldstate = GamePad.GetState(0);
           _state = GamePad.GetState(0);
        }

        public void Update() {
            _oldstate = _state;
            _state = GamePad.GetState(0);            
        }

        /// <summary>
        /// This is provided in case user wants to use an external input manager. 
        /// Pass in the GamePadState of the external input manager to effectively 
        /// "sync" OdgeInput's GamePadState with the external.
        /// </summary>
        /// <param name="padstate">GamePadState of the external input library.</param>
        public void Update(GamePadState padstate) {
            _oldstate = _state;
            _state = padstate;
        }

        public bool isButtonTap(Buttons button) {
            return (_state.IsButtonDown(button) && !_oldstate.IsButtonDown(button));
        }
    }
}
