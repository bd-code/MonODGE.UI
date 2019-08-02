using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Utilities {
    /// <summary>
    /// <para>
    /// Provides support for user input using a Keyboard or GamePad (controller).
    /// </para>
    /// 
    /// <para>
    /// OdgeInput is provided as a stand-alone utility and can be used without
    /// the rest of MonODGE.UI. However, MonODGE.UI is dependent on OdgeInput.
    /// An external input manager can be used for the rest of your game, but 
    /// MonODGE.UI components require an instance of OdgeInput to be updated 
    /// every frame.</para>
    /// </summary>
    public class OdgeInput {
        private static OdgeInput _input = null;

        public static OdgeInput Input {
            get {
                if (_input == null)
                    _input = new OdgeInput();
                return _input;
            }
        }

        private KeyboardHandler _keyboard;
        public static KeyboardHandler KB { get { return Input._keyboard; } }

        private GamePadHandler _gamepads;
        public static GamePadHandler GP { get { return Input._gamepads; } }

        private OdgeInput() {
            _keyboard = new KeyboardHandler();
            _gamepads = new GamePadHandler();
        }


        public void Update() {
            _keyboard.Update();
            _gamepads.Update();
        }
        

        public static bool UP {
            get {
                return (
                    KB.isKeyTap(Keys.Up) || KB.isKeyTap(Keys.W)
                    || GP.isButtonTap(Buttons.DPadUp)
                    || GP.isButtonTap(Buttons.LeftThumbstickUp)
                    );
            }
        }

        public static bool LEFT {
            get {
                return (
                    KB.isKeyTap(Keys.Left) || KB.isKeyTap(Keys.A)
                    || GP.isButtonTap(Buttons.DPadLeft)
                    || GP.isButtonTap(Buttons.LeftThumbstickLeft)
                    );
            }
        }

        public static bool RIGHT {
            get {
                return (
                    KB.isKeyTap(Keys.Right) || KB.isKeyTap(Keys.D)
                    || GP.isButtonTap(Buttons.DPadRight)
                    || GP.isButtonTap(Buttons.LeftThumbstickRight)
                    );
            }
        }

        public static bool DOWN {
            get {
                return (
                    KB.isKeyTap(Keys.Down) || KB.isKeyTap(Keys.S)
                    || GP.isButtonTap(Buttons.DPadDown)
                    || GP.isButtonTap(Buttons.LeftThumbstickDown)
                    );
            }
        }
    }
}
