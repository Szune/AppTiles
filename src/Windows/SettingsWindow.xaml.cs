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
