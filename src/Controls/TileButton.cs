﻿using AppTiles.Commands;
using AppTiles.Input;
using AppTiles.Tiles;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppTiles.Controls
{
    public class TileButton : Button
    {
        private static readonly TransformToAppCommand ToAppCommand = new TransformToAppCommand();
        private static readonly TransformToContainerCommand ToContainerCommand = new TransformToContainerCommand();
        private static readonly TransformToNoteCommand ToNoteCommand = new TransformToNoteCommand();
        private static readonly ExecuteTileCommand ExecuteCommand = new ExecuteTileCommand();
        private static readonly ResetTileCommand ResetCommand = new ResetTileCommand();
        private ITile _tile;
        private readonly Window _window;
        private static readonly ControlTemplate ButtonTemplate;

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
                    Command = new OpenFolderCommand(),
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
                        Command = ToAppCommand,
                        CommandParameter = _tile
                    },
                    new MenuItem
                    {
                        Header = "Container",
                        Command = ToContainerCommand,
                        CommandParameter = _tile
                    },
                    new MenuItem
                    {
                        Header = "Note",
                        Command = ToNoteCommand,
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
            MouseDrag.ResetPosition();
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

            if (MouseDrag.LastSender == null)
            {
                if (!(newTile is AppTile app))
                    return;
                DoDragDropFromFileExplorer(e, app);
                return;
            }

            if (!(e.Data.GetData(typeof(ITile)) is ITile oldTile)) return;

            if (newTile.Id == oldTile.Id)
                return;
            DoDragDropFromTileToTile(newTile, oldTile);
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
            var tempButton = newTile.Button;
            oldTile.Button.Update(newTile);
            tempButton.Update(oldTile);
        }

        private void DoDragDropFromFileExplorer(DragEventArgs e, AppTile newTile)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            var path = paths?.FirstOrDefault() ?? "";
            _window.Activate();
            var result =
                MessageBox.Show($"Do you want to replace the current path '{newTile.Path}' with '{path}'?",
                    "Replacing", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            var text = newTile.Text == "-" ? System.IO.Path.GetFileNameWithoutExtension(path) : newTile.Text;
            newTile.Path = path;
            newTile.Text = text;
            Settings.SetChanged();
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
    }
}
