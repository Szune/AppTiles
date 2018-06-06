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
