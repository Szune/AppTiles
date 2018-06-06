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
