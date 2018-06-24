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
using AppTiles.Helpers;
using AppTiles.Tiles;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AppTiles.Logging;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for TileCollectionWindow.xaml
    /// </summary>
    public partial class TileCollectionWindow : Window
    {
        private readonly TileCollection _tiles;
        private readonly bool _blockSaving;

        public TileCollectionWindow()
        {
            // main window
            Closing += MainWindow_Closing;
            InitializeComponent();
            if(Settings.Current.IsDefault)
                UseDefaultSettings();
            else 
                UseSettings();
            _tiles = Settings.Current.Tiles;
            PathVariables.UseSettings(Settings.Current);
            Settings.SetMainWindow(this);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DebugLogger.Save();
            if (Settings.IsChanged)
                AskToSave();
            Application.Current.Shutdown();
        }

        public TileCollectionWindow(TileCollection tiles)
        {
            // child windows
            InitializeComponent();
            if (tiles.ParentText != null)
                Title = $"AppTiles - {tiles.ParentText}";
            _tiles = tiles;
            CreateWindow(tiles);
            _blockSaving = true;
        }

        public void Rebuild()
        {
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.Children.Clear();
            CreateWindow(_tiles);
        }

        private void CreateWindow(TileCollection tiles)
        {
            Width = tiles.Width;
            Height = tiles.Height;

            for(var i = 0; i < tiles.Columns; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)});
            for(var i = 0; i < tiles.Rows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(1, GridUnitType.Star)});

            for (var row = 0; row < tiles.Rows; row++)
            {
                for (var col = 0; col < tiles.Columns; col++)
                {
                    var tile = tiles.FirstOrDefault(t => t.Column == col && t.Row == row);
                    if (tile == null)
                    {
                        var id = tiles.Count > 0 ? tiles.Max(t => t.Id) + 1 : 0;
                        tile = new AppTile(id, col, row);
                        tiles.Add(tile);
                    }
                    var button = new TileButton(tile, this);
                    MainGrid.Children.Add(button);
                }
            }
        }

        private void UseSettings()
        {
            CreateWindow(Settings.Current.Tiles);
        }

        private void UseDefaultSettings()
        {
            Settings.Current.ResetTiles();
            Settings.Current.Tiles.Add(new AppTile(0, 0, 0, "AppTiles folder", Colors.White, Colors.Black, "{AppFolder}", ""));
            Settings.Current.Tiles.Add(new AppTile(1, 1, 0, "Settings", Colors.White, Colors.Black, "{AppSettings}", ""));
            CreateWindow(Settings.Current.Tiles);
        }

        private void AskToSave()
        {
            if (_blockSaving) return;
            var result = MessageBox.Show($"Do you want to save your changes?", "Saving", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            Settings.Current.Save();
        }

        private void TileCollectionWindow_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                var result = MessageBox.Show("Are you sure you want to reset all tiles?", "Resetting",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;
                for (var i = 0; i < Settings.Current.Tiles.Count; i++)
                    Settings.Current.Tiles[i].Reset();
            }
            else if (e.Key == Key.F6)
            {
                AskToSave();
            }
        }

        private void TileCollectionWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _tiles.Width = (int) e.NewSize.Width;
            _tiles.Height = (int) e.NewSize.Height;
        }
    }
}
