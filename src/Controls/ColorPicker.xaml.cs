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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppTiles.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private bool _mouseDown;
        private Color _currentColor;

        public event EventHandler<ColorPickerEventArgs> ColorChanged;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void ImgColors_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            var pos = e.GetPosition(ImgColors);
            _currentColor = GetPixel((BitmapSource) ImgColors.Source, (int) pos.X, (int) pos.Y);
            ColorChanged?.Invoke(this, new ColorPickerEventArgs(_currentColor));
        }

        private void ImgColors_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
        }

        private void ImgColors_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
                return;
            var pos = e.GetPosition(ImgColors);
            _currentColor = GetPixel((BitmapSource) ImgColors.Source, (int) pos.X, (int) pos.Y);
            ColorChanged?.Invoke(this, new ColorPickerEventArgs(_currentColor));
        }

        private static Color GetPixel(BitmapSource source, int x, int y)
        {
            if (source == null) return Colors.Black;
            try
            {
                var cropped = new CroppedBitmap(source, new Int32Rect(x, y, 1, 1));
                var colorBytes = new byte[4];
                cropped.CopyPixels(colorBytes, 4, 0);
                return Color.FromRgb(colorBytes[2], colorBytes[1], colorBytes[0]);
            }
            catch
            {
                return Colors.Black;
            }
        }

        private void ImgColors_OnIsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // handle releasing mouse button outside of control
            if (Mouse.LeftButton == MouseButtonState.Released)
                _mouseDown = false;
        }
    }
}
