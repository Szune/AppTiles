using System;
using System.Windows;
using System.Windows.Input;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Settings _settings;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            TxtContainerCols.Text = settings.Tiles.Columns.ToString();
            TxtContainerRows.Text = settings.Tiles.Rows.ToString();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            _settings.Tiles.Columns = int.Parse(TxtContainerCols.Text);
            _settings.Tiles.Rows = int.Parse(TxtContainerRows.Text);
            Settings.SetChanged();
            Settings.MainWindow.Rebuild();
            Close();
        }

        private void SettingsWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Update();
            }
            else if (e.Key == Key.Escape)
                Close();
        }

        private void BtnRemoveUnused_OnClick(object sender, RoutedEventArgs e)
        {
            var result =
                MessageBox.Show(
                    $"Are you sure you want to clean settings?{Environment.NewLine}This will remove all invisible tiles from the settings file.",
                    "Cleaning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.No)
                return;

            _settings.Clean();
            _settings.Save();
            Close();
        }
    }
}
