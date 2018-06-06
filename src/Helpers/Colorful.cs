using System;
using System.Linq;
using System.Windows.Media;

namespace AppTiles.Helpers
{
    public static class Colorful
    {
        public static Color String(string hex)
        {
            var alphanumerics = hex.Count(c => char.IsNumber(c) || char.IsLetter(c));
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
