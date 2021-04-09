using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonODGE.UI.Components {
    public class ProgressBar : OdgePopUp {
        private Texture2D _bar;
        private Rectangle barRect;
        private Rectangle invRect;

        private uint _value;
        public uint Value {
            get { return _value; }
            set {
                _value = (uint)MathHelper.Min(value, MaxValue);
                OnValueChanged();
            }
        }

        private uint _maxValue;
        public uint MaxValue {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public float Percent {
            get { return (1.0f * Value) / (1.0f * MaxValue); }
            set {
                if (value >= 0.0f)
                    Value = (uint)(MaxValue * value);
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// If true, draws the incomplete (right) side of the progress bar 
        /// using Style.UnselectedTextColor.
        /// </summary>
        public bool DrawIncompleteBar { get; set; }


        public ProgressBar(StyleSheet style, Rectangle area, Texture2D barTexture, uint maxValue, uint value = 0, bool drawIncomplete = false) 
            : base(style) {
            Timeout = 10;
            _bar = barTexture;
            barRect = Rectangle.Empty;
            invRect = Rectangle.Empty;

            MaxValue = maxValue;
            Value = value;

            Dimensions = area;
            DrawIncompleteBar = drawIncomplete;
        }


        public override void OnMove() {
            recalcBars();
            base.OnMove();
        }
        public override void OnResize() {
            recalcBars();
            base.OnResize();
        }


        public void OnValueChanged() {
            recalcBars();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ValueChanged;


        //public override void Update() { }
        public override void Draw(SpriteBatch batch) {
            // We are NOT drawing the canvas or borders here: only the bars
            batch.Draw(_bar, barRect, Style.SelectedTextColor);
            if (DrawIncompleteBar)
                batch.Draw(_bar, invRect, Style.UnselectedTextColor);
        }


        private void recalcBars() {
            barRect = new Rectangle(
                Dimensions.X, Dimensions.Y, 
                (int)(Dimensions.Width * Percent), 
                Dimensions.Height);

            invRect = new Rectangle(
                barRect.Right + 1,
                Dimensions.Y,
                Dimensions.Width - barRect.Width,
                Dimensions.Height);
        }
    }
}
