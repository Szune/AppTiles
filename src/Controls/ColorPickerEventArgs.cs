using System;
using System.Windows.Media;

namespace AppTiles.Controls
{
    public class ColorPickerEventArgs : EventArgs
    {
        public Color Color { get; }

        public ColorPickerEventArgs(Color color)
        {
            Color = color;
        }
    }
}
