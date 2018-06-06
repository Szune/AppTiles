using System;
using System.Windows;
using System.Windows.Input;
using AppTiles.Tiles;

namespace AppTiles.Commands
{
    public class TransformToAppCommand: ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (parameter is AppTile)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            var result = MessageBox.Show("Are you sure you want to transform this tile?", "Warning!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            if (!(parameter is ITile tile))
            {
                MessageBox.Show($"Could not transform to {nameof(AppTile)}, parameter was of type {parameter.GetType()}.");
                return;
            }

            if (tile.Button == null)
            {
                MessageBox.Show($"Could not transform to {nameof(AppTile)}, not connected to a button yet.");
                return;
            }

            var newTile = new AppTile(tile.Id, tile.Column, tile.Row, tile.Text, tile.Background,
                tile.Foreground, "", "");
            tile.Button.Update(newTile);
            Settings.ReplaceTile(tile, newTile);
            new EditTileCommand().Execute(newTile);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
