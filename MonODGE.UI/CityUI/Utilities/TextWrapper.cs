﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

namespace CityUI.Utilities {
    /// <summary>
    /// Provides static methods for wrapping a string to a specified screenwidth using an XNA/MonoGame SpriteFont.
    /// </summary>
    public class TextWrapper {
        /// <summary>
        /// Wraps a string to a specified char count length.
        /// </summary>
        /// <param name="text">The string to wrap.</param>
        /// <param name="maxLength">The maximum string.length of the wrapped string.</param>
        /// <returns>A new multiline string wrapped at maxLength chars.</returns>
        public static string WrapToCharCount(string text, int maxLength) {
            StringBuilder fullBuilder = new StringBuilder();
            StringBuilder lineBuilder = new StringBuilder();
            string[] splitz = text.Split(' ');

            foreach (string str in splitz) {
                string lineSoFar = lineBuilder.ToString();
                if (lineSoFar.Length + str.Length >= maxLength && lineBuilder.Length > 0) {
                    fullBuilder.AppendLine(lineSoFar);
                    lineBuilder.Clear();
                }

                lineBuilder.Append(str + " ");
            }

            fullBuilder.Append(lineBuilder.ToString());
            return fullBuilder.ToString();
        }

        /// <summary>
        /// Wraps a string to a specified screenwidth using an XNA/MonoGame SpriteFont.
        /// </summary>
        /// <param name="text">The string to wrap.</param>
        /// <param name="font">An XNA/MonoGame SpriteFont to measure the string.</param>
        /// <param name="maxWidth">The maximum screenwidth of the wrapped string.</param>
        /// <returns>A new multiline string wrapped at maxWidth.</returns>
        public static string WrapToWidth(string text, SpriteFont font, float maxWidth) {
            StringBuilder fullBuilder = new StringBuilder();
            StringBuilder lineBuilder = new StringBuilder();
            string[] splitz = text.Split(' ');

            foreach (string str in splitz) {
                string lineSoFar = lineBuilder.ToString();
                if (font.MeasureString(lineSoFar + str).X >= maxWidth && lineBuilder.Length > 0) {
                    fullBuilder.AppendLine(lineSoFar);
                    lineBuilder.Clear();
                }

                lineBuilder.Append(str + " ");
            }

            fullBuilder.Append(lineBuilder.ToString());
            return fullBuilder.ToString();
        }

        /// <summary>
        /// Splits a string into an array of strings, each of specified screenwidth, using an XNA/MonoGame SpriteFont.
        /// </summary>
        /// <param name="text">The string to wrap.</param>
        /// <param name="font">An XNA/MonoGame SpriteFont to measure the string.</param>
        /// <param name="maxWidth">The maximum screenwidth of the wrapped strings.</param>
        /// <returns>An array of strings of length not greater than maxWidth.</returns>
        public static string[] WrapToArray(string text, SpriteFont font, float maxWidth) {
            List<string> textList = new List<string>();
            StringBuilder lineBuilder = new StringBuilder();
            string[] splitz = text.Split(' ');

            foreach (string str in splitz) {
                string lineSoFar = lineBuilder.ToString();
                if (font.MeasureString(lineSoFar + str).X >= maxWidth && lineBuilder.Length > 0) {
                    textList.Add(lineSoFar);
                    lineBuilder.Clear();
                }

                lineBuilder.Append(str + " ");
            }

            textList.Add(lineBuilder.ToString());
            return textList.ToArray<string>();
        }
    }
}
