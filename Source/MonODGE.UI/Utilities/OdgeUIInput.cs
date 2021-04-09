
using Microsoft.Xna.Framework.Input;

namespace MonODGE.UI.Utilities {
    /// <summary>
    /// <para>
    /// Provides support for user input using a Keyboard or GamePad (controller).
    /// </para>
    /// 
    /// <para>
    /// OdgeUIInput is provided as a stand-alone utility and can be used 
    /// without the rest of MonODGE.UI. However, MonODGE.UI is dependent on
    /// OdgeUIInput. An external input manager can be used for the rest of your
    /// game, but MonODGE.UI components require an instance of OdgeUIInput to 
    /// be updated every frame.</para>
    /// </summary>
    public class OdgeUIInput {
        private static OdgeUIInput _input = null;

        public static OdgeUIInput Input {
            get {
                if (_input == null)
                    _input = new OdgeUIInput();
                return _input;
            }
        }

        private KeyboardHandler _keyboard;
        public static KeyboardHandler KB { get { return Input._keyboard; } }

        private GamePadHandler _gamepads;
        public static GamePadHandler GP { get { return Input._gamepads; } }

        private OdgeUIInput() {
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
