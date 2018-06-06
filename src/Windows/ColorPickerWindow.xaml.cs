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
using AppTiles.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        public Color Color { get; private set; }
        public ColorPickerWindow(string parentText, Color startColor)
        {
            InitializeComponent();
            Title = "Color Picker - " + parentText;
            Color = startColor;
            ImgColor.Background = new SolidColorBrush(startColor);
        }

        private void ColorPicker_OnColorChanged(object sender, ColorPickerEventArgs e)
        {
            Color = e.Color;
            ImgColor.Background = new SolidColorBrush(e.Color);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            DialogResult = true;
            Close();
        }

        public static Color? ShowDialog(string parentText, Color startColor)
        {
            var picker = new ColorPickerWindow(parentText, startColor);
            if (picker.ShowDialog() ?? false)
                return picker.Color;
            return null;
        }

        public static Color? ShowDialog(string parentText)
        {
            var picker = new ColorPickerWindow(parentText, Colors.Black);
            if (picker.ShowDialog() ?? false)
                return picker.Color;
            return null;
        }

        private void ColorPickerWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Update();
            }
            else if (e.Key == Key.Escape)
                Close();
        }
    }
}
