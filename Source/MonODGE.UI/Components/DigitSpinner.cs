﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonODGE.UI.Utilities;

namespace MonODGE.UI.Components {
    public class DigitSpinner : OdgeControl {
        private int _value;
        public int Value {
            get { return _value; }
            set { _value = MathHelper.Clamp(value, MinValue, MaxValue); }
        }

        public int Step { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        protected override int MinHeight {
            get {
                return Style.PaddingBottom + Style.PaddingTop + (int)(textDimensions[0].Y);
            }
        }

        private Vector2[] textPositions;
        private Vector2[] textDimensions;

        public DigitSpinner(StyleSheet style, Rectangle area, 
        int minVal = 0, int maxVal = int.MaxValue, int step = 1, int initval = 0) : base(style) {
            MinValue = minVal;
            MaxValue = maxVal;
            Step = step;
            Value = initval;

            textPositions = new Vector2[3];
            textDimensions = new Vector2[] {
                Style.Font?.MeasureString("<<") ?? Vector2.Zero,
                Style.Font?.MeasureString(initval.ToString()) ?? Vector2.Zero,
                Style.Font?.MeasureString(">>") ?? Vector2.Zero
            };
            
            Dimensions = area;
        }


        public override void OnStyleChanged() {
            textDimensions = new Vector2[] {
                Style.Font?.MeasureString("<<") ?? Vector2.Zero,
                Style.Font?.MeasureString(_value.ToString()) ?? Vector2.Zero,
                Style.Font?.MeasureString(">>") ?? Vector2.Zero
            };
            repositionText();
            base.OnStyleChanged();
        }
        public override void OnMove() {
            repositionText();
            base.OnMove();
        }
        public override void OnResize() {
            repositionText();
            base.OnResize();
        }


        /// <summary>
        /// This is called when Value is incremented by pressing Keys.Right.
        /// </summary>
        public void OnIncrement() { Increment?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Increment;


        /// <summary>
        /// This is called when Value is decremented by pressing Keys.Left.
        /// </summary>
        public void OnDecrement() { Decrement?.Invoke(this, EventArgs.Empty); }
        public event EventHandler Decrement;


        public override void Update() {
            if (CheckSubmit) {
                OnSubmit();
            }

            if ((OdgeUIInput.LEFT || OdgeUIInput.DOWN) && Value-Step >= MinValue) {
                DecrementByStep();
            }

            else if ((OdgeUIInput.RIGHT || OdgeUIInput.UP) && Value+Step <= MaxValue) {
                IncrementByStep();
            }

            else if (CheckCancel) {
                OnCancel();
            }
        }


        public override void Draw(SpriteBatch batch) {
            DrawBG(batch);
            DrawBorders(batch);

            if (Value > MinValue)
                batch.DrawString(Style.FooterFont, "<<", textPositions[0], Style.FooterColor);

            batch.DrawString(Style.Font, Value.ToString(), textPositions[1], Style.TextColor);

            if (Value < MaxValue)
                batch.DrawString(Style.FooterFont, ">>", textPositions[2], Style.FooterColor);
        }


        public void IncrementBy(int val) {
            Value += val;
            OnIncrement();
            textDimensions[1] = Style.Font?.MeasureString(Value.ToString()) ?? Vector2.Zero;
        }


        public void DecrementBy(int val) {
            Value -= val;
            OnDecrement();
            textDimensions[1] = Style.Font?.MeasureString(Value.ToString()) ?? Vector2.Zero;
        }


        public void IncrementByStep() { IncrementBy(Step); }
        public void DecrementByStep() { DecrementBy(Step); }


        private void repositionText() {
            float ny;

            // Vertical Positioning Only
            if (Style.TextAlignV == StyleSheet.AlignmentsV.TOP)
                ny = Y + Style.PaddingTop;
            else if (Style.TextAlignV == StyleSheet.AlignmentsV.CENTER)
                ny = Dimensions.Center.Y - (textDimensions[0].Y / 2);
            else // Bottom
                ny = Dimensions.Bottom - textDimensions[0].Y - Style.PaddingBottom;
            

            textPositions[0] = new Vector2(X + Style.PaddingLeft, ny);
            textPositions[1] = new Vector2(Dimensions.Center.X - (textDimensions[1].X / 2), ny);
            textPositions[2] = new Vector2(Dimensions.Right - textDimensions[2].X - Style.PaddingRight, ny);
        }
    }
}
