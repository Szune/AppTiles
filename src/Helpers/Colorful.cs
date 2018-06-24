#region License & Terms
// MIT License

// Copyright (c) 2018 Erik Iwarson

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
#endregion
using System;
using System.Linq;
using System.Windows.Media;

namespace AppTiles.Helpers
{
    public static class Colorful
    {
        public static Color String(string hex)
        {
            var alphanumerics = hex.Count(char.IsLetterOrDigit);
            switch (alphanumerics)
            {
                case 8:
                {
                    var bytes = FromHex(hex, 4);
                    return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
                }
                case 6:
                {
                    var bytes = FromHex(hex, 3);
                    return Color.FromRgb(bytes[0], bytes[1], bytes[2]);
                }
                default:
                    throw new ArgumentException("8 or 6 alphanumeric characters are required to build a color.");
            }
        }


        private static byte[] FromHex(string hex, int byteAmount)
        {
            var hexStr = hex.Replace("#", "").Trim().ToUpperInvariant();
            var bytes = new byte[byteAmount];
            for (var i = 0; i < bytes.Length; i++)
            {
                var twoLetters = hexStr.Substring(i * 2, 2);
                bytes[i] = (byte) (HexToInt(twoLetters[0]) * 16 + HexToInt(twoLetters[1]));
            }

            return bytes;
        }

        private static int HexToInt(char hex)
        {
            if (hex >= '0' && hex <= '9')
                return hex - 48;
            if (hex >= 'A' && hex <= 'F')
                return hex - 55;
            if (hex >= 'a' && hex <= 'f')
                return hex - 87;

            throw new ArgumentException("Expected hex value (0-F).");
        }
    }
}
