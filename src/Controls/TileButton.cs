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
using AppTiles.Commands;
using AppTiles.Input;
using AppTiles.Tiles;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AppTiles.Helpers;
#if DEBUG
using AppTiles.Logging;
#endif

namespace AppTiles.Controls
{
    public class TileButton : Button
    {
        private static readonly ExecuteTileCommand ExecuteCommand = new ExecuteTileCommand();
        private static readonly ResetTileCommand ResetCommand = new ResetTileCommand();
        private ITile _tile;
        private readonly Window _window;
        private static readonly ControlTemplate ButtonTemplate;

        private readonly OpenFolderCommand _openFolderCommand = new OpenFolderCommand();
        private readonly TransformToAppCommand _toAppCommand = new TransformToAppCommand();
        private readonly TransformToContainerCommand _toContainerCommand = new TransformToContainerCommand();
        private readonly TransformToNoteCommand _toNoteCommand = new TransformToNoteCommand();

        static TileButton()
        {
            ButtonTemplate = (ControlTemplate)Application.Current.FindResource("TileButton");
        }

        public TileButton(ITile tile, Window window)
        {
            _tile = tile;
            _window = window;
            CreateButton();
        }
        public void Update(ITile tile)
        {
            _tile = tile;
            CommandParameter = _tile;
            ContextMenu = CreateContextMenu();
            _tile.SetButton(this);
            RefreshButtonLayout();
        }

        public void UpdateText(string text)
        {
            if (Content is TextBlock tb)
            {
                tb.Text = text;
            }
            else
            {
                Content = text;
            }
        }

        public void RefreshCommands()
        {
            _openFolderCommand.Refresh();
            _toAppCommand.Refresh();
            _toContainerCommand.Refresh();
            _toNoteCommand.Refresh();
        }

        private void RefreshButtonLayout()
        {
            UpdateText(_tile.Text);
            Background = new SolidColorBrush(_tile.Background);
            Foreground = new SolidColorBrush(_tile.Foreground);
        }

        private void CreateButton()
        {
            var contextMenu = CreateContextMenu();
            var textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Text = _tile.Text
            };

            Command = ExecuteCommand;
            CommandParameter = _tile;
            Content = textBlock;
            Background = new SolidColorBrush(_tile.Background);
            Foreground = new SolidColorBrush(_tile.Foreground);
            ContextMenu = contextMenu;
            AllowDrop = true;
            Template = ButtonTemplate;
            Drop += Button_Drop;
            PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
            PreviewMouseMove += Button_PreviewMouseMove;

            _tile.SetButton(this);
            Grid.SetColumn(this, _tile.Column);
            Grid.SetRow(this, _tile.Row);
        }
        
        private ContextMenu CreateContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem
            {
                Header = "Edit",
                Command = new EditTileCommand(),
                CommandParameter = _tile,
            });

            if (_tile is AppTile) // only app tiles make sense with opening folders
            {
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Open folder",
                    Command = _openFolderCommand,
                    CommandParameter = _tile,
                });
            }

            // context menu for transforming to a different type of tile
            contextMenu.Items.Add(new MenuItem
            {
                Header = "Transform to",
                Items =
                {
                    new MenuItem
                    {
                        Header = "App",
                        Command = _toAppCommand,
                        CommandParameter = _tile
                    },
                    new MenuItem
                    {
                        Header = "Container",
                        Command = _toContainerCommand,
                        CommandParameter = _tile
                    },
                    new MenuItem
                    {
                        Header = "Note",
                        Command = _toNoteCommand,
                        CommandParameter = _tile
                    }
                }
            });

            contextMenu.Items.Add(new MenuItem
            {
                Header = "Reset",
                Command = ResetCommand,
                CommandParameter = _tile,
            });

            return contextMenu;
        }


        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseDrag.IsBeingPerformed)
                return;
            var pos = e.GetPosition(this);
            if (!MouseDrag.HasMoved(pos))
                return;
            new MoveTileCommand().Execute(MouseDrag.LastSender._tile);
            MouseDrag.ResetPosition();
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseDrag.ResetAll();
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseDrag.StartNew(sender as TileButton, e.GetPosition(this));
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (!(sender is TileButton newButton))
                return;
            if (!(newButton._tile is ITile newTile))
                return;

            #if DEBUG
            CheckDragData(e.Data);
            #endif

            if (!MouseDrag.IsBeingPerformed) // if dragging from file explorer, we won't be notified until it's dropped
            {
                if (!(newTile is AppTile app))
                    return;
                // file
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    DoDragDropFromFileExplorer(e, app);
                // url (chrome, does not work for ms edge)
                else if (e.Data.GetDataPresent(DataFormats.Text))
                    DoDragDropFromWebBrowser(e, app);
                return;
            }

            if (!(e.Data.GetData(typeof(ITile)) is ITile oldTile)) return;

            if (newTile.Id == oldTile.Id)
                return;
            DoDragDropFromTileToTile(newTile, oldTile);
        }

        private void DoDragDropFromWebBrowser(DragEventArgs e, AppTile newTile)
        {
            var url = e.Data.GetData(DataFormats.Text) as string ?? "";
            var resolver = new PathResolver(url);
            if (resolver.Type != PathType.Web && resolver.Type != PathType.UnknownProtocol)
                return;
            var host = new Uri(url).Host;
            _window.Activate();
            var result =
                MessageBox.Show($"Do you want to replace the current path '{newTile.Path}' with '{url}'?",
                    $"Replacing '{_tile.Text}'", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            var text = newTile.Text == "-" ? host : newTile.Text;
            newTile.Path = url;
            newTile.Text = text;
            Settings.SetChanged();
            MouseDrag.ResetAll();
        }

        private void DoDragDropFromTileToTile(ITile newTile, ITile oldTile)
        {
            _window.Activate();
            var result = MessageBox.Show($"Do you want to replace '{newTile.Text}' with '{oldTile.Text}'?",
                "Replacing", MessageBoxButton.YesNo, MessageBoxImage.Question);
            MouseDrag.ResetAll();
            if (result == MessageBoxResult.No)
                return;
            Settings.SetChanged();
            int tempCol = newTile.Column, tempRow = newTile.Row;
            var tempButton = newTile.Button;
            // update the buttons' references
            oldTile.Button.Update(newTile);
            tempButton.Update(oldTile);
            // update the tiles' saved location
            newTile.Move(oldTile.Column, oldTile.Row);
            oldTile.Move(tempCol, tempRow);

            MouseDrag.ResetAll();
        }

        private void DoDragDropFromFileExplorer(DragEventArgs e, AppTile newTile)
        {
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            var path = paths?.FirstOrDefault() ?? "";
            _window.Activate();
            var result =
                MessageBox.Show($"Do you want to replace the current path '{newTile.Path}' with '{path}'?",
                    $"Replacing '{_tile.Text}'", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            var text = newTile.Text == "-" ? System.IO.Path.GetFileNameWithoutExtension(path) : newTile.Text;
            newTile.Path = path;
            newTile.Text = text;
            Settings.SetChanged();
            MouseDrag.ResetAll();
        }

#if DEBUG
        private void CheckDragData(IDataObject data)
        {
            // only ever used for figuring out how dragdrop-data is represented
            var dataFormats = new[]
            {
                DataFormats.StringFormat,
                DataFormats.Text,
                DataFormats.Html,
                DataFormats.OemText,
                DataFormats.UnicodeText,
                DataFormats.CommaSeparatedValue,
                DataFormats.Dif,
                DataFormats.FileDrop,
                DataFormats.EnhancedMetafile,
                DataFormats.Locale,
                DataFormats.Rtf,
                DataFormats.Serializable,
                DataFormats.Xaml,
                DataFormats.XamlPackage,
                DataFormats.SymbolicLink
            };

            foreach (var format in dataFormats)
            {
                var fetched = data.GetData(format);
                DebugLogger.WriteLine($"DragDrop data ({format}): {fetched}");
            }
        }
#endif
    }
}
